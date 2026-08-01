import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { AdminPanel } from '../AdminPanel';

const { apiGet, apiPost, apiPut, apiDelete, mockProjects, mockPosts } = vi.hoisted(() => {
  const projects = [
    { id: 1, slug: 'tutor-finder', title: 'Tutor Finder', description: 'A tutoring platform.', imageUrl: '/a.png', categories: ['webdesign', 'webapp'], technologies: ['React', 'Node.js'], displayOrder: 1 },
    { id: 2, slug: 'college-lake', title: 'CollegeLake', description: 'College discovery app.', imageUrl: '/b.png', categories: ['mobiledesign'], technologies: ['React Native'], displayOrder: 2 },
  ];

  const posts = [
    { id: 1, slug: 'hello-admin', title: 'Hello Admin', excerpt: 'A useful article.', content: '', imageUrl: '/blog.png', publishedAt: '2026-07-30T00:00:00Z', author: 'Satyam Kumar', tags: ['Admin'], isPublished: true },
  ];

  return {
    apiGet: vi.fn(),
    apiPost: vi.fn(),
    apiPut: vi.fn(),
    apiDelete: vi.fn(),
    mockProjects: projects,
    mockPosts: posts,
  };
});

vi.mock('../../../api/client', () => ({
  apiClient: {
    get: apiGet,
    post: apiPost,
    put: apiPut,
    delete: apiDelete,
  },
}));

function wrap(ui: React.ReactElement) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(<QueryClientProvider client={qc}>{ui}</QueryClientProvider>);
}

async function signIn() {
  const user = userEvent.setup();
  wrap(<AdminPanel />);

  await user.type(screen.getByLabelText(/Password/i), '$Atyam@100.');
  await user.click(screen.getByRole('button', { name: /Sign in/i }));

  await screen.findByRole('heading', { name: /Admin workspace/i });
  return user;
}

describe('AdminPanel', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    window.localStorage.clear();

    apiGet.mockImplementation((url: string) => {
      if (url === '/api/projects') return Promise.resolve({ data: mockProjects });
      if (url === '/api/blog') return Promise.resolve({ data: mockPosts });
      return Promise.reject(new Error(`Unhandled GET ${url}`));
    });

    apiPost.mockImplementation((url: string) => {
      if (url === '/api/godmode/login') {
        return Promise.resolve({
          data: {
            token: 'admin-token',
            email: 'johndoeunique101@gmail.com',
            expiresAt: new Date(Date.now() + 60_000).toISOString(),
          },
        });
      }

      if (url === '/api/projects') return Promise.resolve({ data: mockProjects[0] });
      if (url === '/api/blog') return Promise.resolve({ data: mockPosts[0] });
      return Promise.reject(new Error(`Unhandled POST ${url}`));
    });

    apiPut.mockResolvedValue({ data: mockProjects[0] });
    apiDelete.mockResolvedValue({ data: undefined });
  });

  it('renders the godmode login first', () => {
    wrap(<AdminPanel />);
    expect(screen.getByRole('heading', { name: /Admin login/i })).toBeInTheDocument();
    expect(screen.queryByRole('heading', { name: /Admin workspace/i })).not.toBeInTheDocument();
  });

  it('requires a password before login', async () => {
    const user = userEvent.setup();
    wrap(<AdminPanel />);

    await user.click(screen.getByRole('button', { name: /Sign in/i }));

    await waitFor(() => {
      expect(screen.getByText(/Enter the admin password/i)).toBeInTheDocument();
    });
  });

  it('loads projects after login', async () => {
    await signIn();

    expect(screen.getByText('Tutor Finder')).toBeInTheDocument();
    expect(screen.getByText('CollegeLake')).toBeInTheDocument();
    expect(screen.getByText('2 projects')).toBeInTheDocument();
  });

  it('shows blog posts in the blog tab', async () => {
    const user = await signIn();

    await user.click(screen.getByRole('tab', { name: /Blog/i }));

    await waitFor(() => {
      expect(screen.getByText('Hello Admin')).toBeInTheDocument();
      expect(screen.getByText('1 posts')).toBeInTheDocument();
    });
  });

  it('posts new projects through the admin API', async () => {
    const user = await signIn();

    await user.click(screen.getByRole('button', { name: /Create Project/i }));
    await user.type(screen.getByLabelText('Title'), 'New Project');
    await user.type(screen.getByLabelText('Description'), 'A detailed project description.');
    await user.type(screen.getByLabelText('Technologies'), 'React, PostgreSQL');
    await user.click(screen.getByRole('button', { name: /^Save$/i }));

    await waitFor(() => {
      expect(apiPost).toHaveBeenCalledWith(
        '/api/projects',
        expect.objectContaining({
          title: 'New Project',
          categories: ['webdesign'],
          technologies: ['React', 'PostgreSQL'],
        }),
        expect.objectContaining({
          headers: expect.objectContaining({ Authorization: 'Bearer admin-token' }),
        })
      );
    });
  });

  it('posts new blog posts through the admin API', async () => {
    const user = await signIn();

    await user.click(screen.getByRole('tab', { name: /Blog/i }));
    await user.click(screen.getByRole('button', { name: /Create Blog Post/i }));
    await user.type(screen.getByLabelText('Title'), 'New Blog Post');
    await user.type(screen.getByLabelText('Excerpt'), 'A useful article summary.');
    await user.type(screen.getByLabelText('Content'), 'Full article content for the admin editor.');
    await user.type(screen.getByLabelText('Tags'), 'React, Admin');
    await user.click(screen.getByRole('button', { name: /^Save$/i }));

    await waitFor(() => {
      expect(apiPost).toHaveBeenCalledWith(
        '/api/blog',
        expect.objectContaining({
          title: 'New Blog Post',
          excerpt: 'A useful article summary.',
          tags: ['React', 'Admin'],
        }),
        expect.objectContaining({
          headers: expect.objectContaining({ Authorization: 'Bearer admin-token' }),
        })
      );
    });
  });

  it('confirms deletes before calling the API', async () => {
    const user = await signIn();

    await user.click(screen.getByRole('button', { name: /Delete Tutor Finder/i }));
    expect(apiDelete).not.toHaveBeenCalled();

    await user.click(screen.getByRole('button', { name: /^Delete$/i }));

    await waitFor(() => {
      expect(apiDelete).toHaveBeenCalledWith(
        '/api/projects/1',
        expect.objectContaining({
          headers: expect.objectContaining({ Authorization: 'Bearer admin-token' }),
        })
      );
    });
  });

  it('confirms blog deletes before calling the API', async () => {
    const user = await signIn();

    await user.click(screen.getByRole('tab', { name: /Blog/i }));
    await user.click(screen.getByRole('button', { name: /Delete Hello Admin/i }));
    expect(apiDelete).not.toHaveBeenCalled();

    await user.click(screen.getByRole('button', { name: /^Delete$/i }));

    await waitFor(() => {
      expect(apiDelete).toHaveBeenCalledWith(
        '/api/blog/1',
        expect.objectContaining({
          headers: expect.objectContaining({ Authorization: 'Bearer admin-token' }),
        })
      );
    });
  });
});
