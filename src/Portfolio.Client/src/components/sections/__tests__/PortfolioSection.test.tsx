import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { describe, expect, it, vi } from 'vitest';
import { PortfolioSection } from '../PortfolioSection';

const { useProjectsMock } = vi.hoisted(() => {
  const projects = [
    { id: 1, slug: 'project-a', title: 'Project A', description: 'A web project', imageUrl: '/a.png', categories: ['webdesign'], technologies: ['React'], displayOrder: 1 },
    { id: 2, slug: 'project-b', title: 'Project B', description: 'A mobile app', imageUrl: '/b.png', categories: ['mobiledesign', 'webapp'], technologies: ['React Native'], displayOrder: 2 },
    { id: 3, slug: 'project-c', title: 'Project C', description: 'A game', imageUrl: '/c.png', categories: ['gamedesign'], technologies: ['Unity'], displayOrder: 3 },
  ];

  return {
    useProjectsMock: vi.fn((category?: string) => ({
      data: !category || category === '*'
        ? projects
        : projects.filter((project) => project.categories.includes(category)),
      isLoading: false,
    })),
  };
});

vi.mock('../../../api/queries', () => ({
  useProjects: (category?: string) => useProjectsMock(category),
}));

function wrap(ui: React.ReactElement) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(<QueryClientProvider client={qc}>{ui}</QueryClientProvider>);
}

describe('PortfolioSection', () => {
  it('renders all projects by default', () => {
    wrap(<PortfolioSection />);
    expect(screen.getByText('Project A')).toBeInTheDocument();
    expect(screen.getByText('Project B')).toBeInTheDocument();
    expect(screen.getByText('Project C')).toBeInTheDocument();
  });

  it('renders filter buttons', () => {
    wrap(<PortfolioSection />);
    expect(screen.getByRole('tab', { name: 'All' })).toBeInTheDocument();
    expect(screen.getByRole('tab', { name: 'Web Design' })).toBeInTheDocument();
    expect(screen.getByRole('tab', { name: 'Mobile' })).toBeInTheDocument();
  });

  it('marks All filter as selected by default', () => {
    wrap(<PortfolioSection />);
    expect(screen.getByRole('tab', { name: 'All' })).toHaveAttribute('aria-selected', 'true');
  });

  it('updates active tab when filter clicked', async () => {
    const user = userEvent.setup();
    wrap(<PortfolioSection />);

    const webTab = screen.getByRole('tab', { name: 'Web Design' });
    await user.click(webTab);

    await waitFor(() => {
      expect(webTab).toHaveAttribute('aria-selected', 'true');
      expect(screen.getByRole('tab', { name: 'All' })).toHaveAttribute('aria-selected', 'false');
      expect(useProjectsMock).toHaveBeenCalledWith('webdesign');
    });
  });

  it('section heading is present and accessible', () => {
    wrap(<PortfolioSection />);
    expect(screen.getByRole('heading', { name: /Featured projects/i })).toBeInTheDocument();
  });

  it('shows project count', () => {
    wrap(<PortfolioSection />);
    expect(screen.getByText(/3 projects/i)).toBeInTheDocument();
  });
});
