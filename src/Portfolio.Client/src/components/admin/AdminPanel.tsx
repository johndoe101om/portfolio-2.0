import { useEffect, useMemo, useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  Check,
  FolderKanban,
  LogIn,
  LogOut,
  Newspaper,
  Pencil,
  Plus,
  RefreshCw,
  Save,
  Search,
  Trash2,
  X,
} from 'lucide-react';
import { apiClient } from '../../api/client';
import type { BlogPost, Project, ProjectCategory } from '../../types';
import styles from './AdminPanel.module.css';

const SESSION_KEY = 'portfolio_admin_session';

const CATEGORIES: { value: ProjectCategory; label: string }[] = [
  { value: 'webdesign', label: 'Web Design' },
  { value: 'webapp', label: 'Web App' },
  { value: 'mobiledesign', label: 'Mobile' },
  { value: 'gamedesign', label: 'Game' },
];

const CATEGORY_LABEL: Record<ProjectCategory, string> = {
  webdesign: 'Web Design',
  webapp: 'Web App',
  mobiledesign: 'Mobile',
  gamedesign: 'Game',
};

type AdminSession = {
  token: string;
  email: string;
  expiresAt: string;
};

type Toast = { message: string; type: 'success' | 'error' | 'info' };

const loginSchema = z.object({
  email: z.string().email('Enter the admin email'),
  password: z.string().min(1, 'Enter the admin password'),
});

const projectSchema = z.object({
  title: z.string().min(2, 'Title is required'),
  description: z.string().min(10, 'Description is too short'),
  imageUrl: z.string().min(1, 'Image URL is required'),
  categories: z.array(z.string()).min(1, 'Select at least one category'),
  technologies: z.string().min(1, 'Add at least one technology'),
  liveUrl: z.string().url('Enter a valid URL').optional().or(z.literal('')),
  displayOrder: z.coerce.number().int().min(0),
});

const blogSchema = z.object({
  title: z.string().min(2, 'Title is required'),
  excerpt: z.string().min(10, 'Excerpt is too short'),
  content: z.string().optional(),
  imageUrl: z.string().min(1, 'Image URL is required'),
  publishedAt: z.string().min(1, 'Publish date is required'),
  author: z.string().min(2, 'Author is required'),
  tags: z.string().optional(),
  isPublished: z.boolean(),
});

type LoginValues = z.infer<typeof loginSchema>;
type ProjectValues = z.infer<typeof projectSchema>;
type BlogValues = z.infer<typeof blogSchema>;
type DeleteTarget = { type: 'project' | 'blog'; id: number } | null;

const defaultProjectValues: ProjectValues = {
  title: '',
  description: '',
  imageUrl: '/assets/images/placeholder.png',
  categories: [],
  technologies: '',
  liveUrl: '',
  displayOrder: 0,
};

const defaultBlogValues: BlogValues = {
  title: '',
  excerpt: '',
  content: '',
  imageUrl: '/assets/images/placeholder.png',
  publishedAt: toDateTimeLocal(new Date().toISOString()),
  author: 'Satyam Kumar',
  tags: '',
  isPublished: true,
};

function readSession(): AdminSession | null {
  if (typeof window === 'undefined') return null;

  try {
    const raw = window.localStorage.getItem(SESSION_KEY);
    if (!raw) return null;

    const parsed = JSON.parse(raw) as AdminSession;
    if (!parsed.token || new Date(parsed.expiresAt).getTime() <= Date.now()) {
      window.localStorage.removeItem(SESSION_KEY);
      return null;
    }

    return parsed;
  } catch {
    window.localStorage.removeItem(SESSION_KEY);
    return null;
  }
}

function saveSession(session: AdminSession | null) {
  if (typeof window === 'undefined') return;

  if (session) {
    window.localStorage.setItem(SESSION_KEY, JSON.stringify(session));
  } else {
    window.localStorage.removeItem(SESSION_KEY);
  }
}

function authHeaders(token: string) {
  return { Authorization: `Bearer ${token}` };
}

function splitCsv(value?: string) {
  return (value ?? '')
    .split(',')
    .map((item) => item.trim())
    .filter(Boolean);
}

function toDateTimeLocal(value: string) {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return '';

  const offsetMs = date.getTimezoneOffset() * 60_000;
  return new Date(date.getTime() - offsetMs).toISOString().slice(0, 16);
}

function fromDateTimeLocal(value: string) {
  return new Date(value).toISOString();
}

function toProjectPayload(values: ProjectValues) {
  return {
    title: values.title,
    description: values.description,
    imageUrl: values.imageUrl,
    categories: values.categories,
    technologies: splitCsv(values.technologies),
    liveUrl: values.liveUrl || null,
    displayOrder: values.displayOrder,
  };
}

function toBlogPayload(values: BlogValues) {
  return {
    title: values.title,
    excerpt: values.excerpt,
    content: values.content || '',
    imageUrl: values.imageUrl,
    publishedAt: fromDateTimeLocal(values.publishedAt),
    author: values.author,
    tags: splitCsv(values.tags),
    isPublished: values.isPublished,
  };
}

export function AdminPanel() {
  const queryClient = useQueryClient();
  const [session, setSession] = useState<AdminSession | null>(() => readSession());
  const [activeTab, setActiveTab] = useState<'projects' | 'blog'>('projects');
  const [toast, setToast] = useState<Toast | null>(null);
  const [search, setSearch] = useState('');
  const [editingProjectId, setEditingProjectId] = useState<number | null>(null);
  const [editingBlogId, setEditingBlogId] = useState<number | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<DeleteTarget>(null);

  const loginForm = useForm<LoginValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: 'johndoeunique101@gmail.com', password: '' },
  });

  const projectForm = useForm<ProjectValues>({
    resolver: zodResolver(projectSchema),
    defaultValues: defaultProjectValues,
  });

  const blogForm = useForm<BlogValues>({
    resolver: zodResolver(blogSchema),
    defaultValues: defaultBlogValues,
  });

  const token = session?.token ?? '';

  const projectsQuery = useQuery<Project[]>({
    queryKey: ['admin', 'projects'],
    enabled: Boolean(token),
    queryFn: async () => {
      const res = await apiClient.get('/api/projects', { headers: authHeaders(token) });
      return res.data;
    },
  });

  const blogQuery = useQuery<BlogPost[]>({
    queryKey: ['admin', 'blog'],
    enabled: Boolean(token),
    queryFn: async () => {
      const res = await apiClient.get('/api/blog', {
        params: { page: 1, pageSize: 50, includeUnpublished: true },
        headers: authHeaders(token),
      });
      return res.data;
    },
  });

  const projects = projectsQuery.data ?? [];
  const posts = blogQuery.data ?? [];

  const filteredProjects = useMemo(() => {
    const term = search.trim().toLowerCase();
    if (!term) return projects;
    return projects.filter((project) =>
      [project.title, project.description, project.slug].some((value) =>
        value.toLowerCase().includes(term)
      )
    );
  }, [projects, search]);

  const filteredPosts = useMemo(() => {
    const term = search.trim().toLowerCase();
    if (!term) return posts;
    return posts.filter((post) =>
      [post.title, post.excerpt, post.slug, post.author].some((value) =>
        value.toLowerCase().includes(term)
      )
    );
  }, [posts, search]);

  function showToast(message: string, type: Toast['type'] = 'success') {
    setToast({ message, type });
    window.setTimeout(() => setToast(null), 3000);
  }

  function handleSessionExpired() {
    setSession(null);
    saveSession(null);
    showToast('Admin session expired', 'error');
  }

  const loginMutation = useMutation({
    mutationFn: async (values: LoginValues) => {
      const res = await apiClient.post('/api/godmode/login', values);
      return res.data as AdminSession;
    },
    onSuccess: (nextSession) => {
      setSession(nextSession);
      saveSession(nextSession);
      showToast('Signed in to godmode');
    },
    onError: (error: Error) => showToast(error.message, 'error'),
  });

  const createProject = useMutation({
    mutationFn: async (values: ProjectValues) => {
      const res = await apiClient.post('/api/projects', toProjectPayload(values), {
        headers: authHeaders(token),
      });
      return res.data as Project;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['admin', 'projects'] });
      await queryClient.invalidateQueries({ queryKey: ['projects'] });
      resetProjectForm();
      showToast('Project created');
    },
    onError: (error: Error) => showToast(error.message, 'error'),
  });

  const updateProject = useMutation({
    mutationFn: async ({ id, values }: { id: number; values: ProjectValues }) => {
      const res = await apiClient.put(`/api/projects/${id}`, toProjectPayload(values), {
        headers: authHeaders(token),
      });
      return res.data as Project;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['admin', 'projects'] });
      await queryClient.invalidateQueries({ queryKey: ['projects'] });
      resetProjectForm();
      showToast('Project updated');
    },
    onError: (error: Error) => showToast(error.message, 'error'),
  });

  const deleteProject = useMutation({
    mutationFn: async (id: number) => {
      await apiClient.delete(`/api/projects/${id}`, { headers: authHeaders(token) });
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['admin', 'projects'] });
      await queryClient.invalidateQueries({ queryKey: ['projects'] });
      setDeleteTarget(null);
      showToast('Project deleted', 'info');
    },
    onError: (error: Error) => showToast(error.message, 'error'),
  });

  const createBlog = useMutation({
    mutationFn: async (values: BlogValues) => {
      const res = await apiClient.post('/api/blog', toBlogPayload(values), {
        headers: authHeaders(token),
      });
      return res.data as BlogPost;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['admin', 'blog'] });
      await queryClient.invalidateQueries({ queryKey: ['blog'] });
      resetBlogForm();
      showToast('Blog post created');
    },
    onError: (error: Error) => showToast(error.message, 'error'),
  });

  const updateBlog = useMutation({
    mutationFn: async ({ id, values }: { id: number; values: BlogValues }) => {
      const res = await apiClient.put(`/api/blog/${id}`, toBlogPayload(values), {
        headers: authHeaders(token),
      });
      return res.data as BlogPost;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['admin', 'blog'] });
      await queryClient.invalidateQueries({ queryKey: ['blog'] });
      resetBlogForm();
      showToast('Blog post updated');
    },
    onError: (error: Error) => showToast(error.message, 'error'),
  });

  const deleteBlog = useMutation({
    mutationFn: async (id: number) => {
      await apiClient.delete(`/api/blog/${id}`, { headers: authHeaders(token) });
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['admin', 'blog'] });
      await queryClient.invalidateQueries({ queryKey: ['blog'] });
      setDeleteTarget(null);
      showToast('Blog post deleted', 'info');
    },
    onError: (error: Error) => showToast(error.message, 'error'),
  });

  useEffect(() => {
    if (!session || (!projectsQuery.error && !blogQuery.error)) return;

    const message = (projectsQuery.error ?? blogQuery.error)?.message ?? '';
    if (message.toLowerCase().includes('admin') || message.toLowerCase().includes('401')) {
      handleSessionExpired();
    }
  }, [blogQuery.error, projectsQuery.error, session]);

  function logout() {
    setSession(null);
    saveSession(null);
    showToast('Signed out', 'info');
  }

  function resetProjectForm() {
    setEditingProjectId(null);
    projectForm.reset({
      ...defaultProjectValues,
      displayOrder: projects.length + 1,
    });
  }

  function resetBlogForm() {
    setEditingBlogId(null);
    blogForm.reset(defaultBlogValues);
  }

  function startProjectEdit(project: Project) {
    setActiveTab('projects');
    setEditingProjectId(project.id);
    projectForm.reset({
      title: project.title,
      description: project.description,
      imageUrl: project.imageUrl,
      categories: project.categories,
      technologies: project.technologies.join(', '),
      liveUrl: project.liveUrl ?? '',
      displayOrder: project.displayOrder,
    });
  }

  function startBlogEdit(post: BlogPost) {
    setActiveTab('blog');
    setEditingBlogId(post.id);
    blogForm.reset({
      title: post.title,
      excerpt: post.excerpt,
      content: post.content ?? '',
      imageUrl: post.imageUrl,
      publishedAt: toDateTimeLocal(post.publishedAt),
      author: post.author,
      tags: post.tags.join(', '),
      isPublished: post.isPublished ?? true,
    });
  }

  function submitProject(values: ProjectValues) {
    if (!token) return handleSessionExpired();

    if (editingProjectId) {
      updateProject.mutate({ id: editingProjectId, values });
    } else {
      createProject.mutate(values);
    }
  }

  function submitBlog(values: BlogValues) {
    if (!token) return handleSessionExpired();

    if (editingBlogId) {
      updateBlog.mutate({ id: editingBlogId, values });
    } else {
      createBlog.mutate(values);
    }
  }

  function confirmDelete() {
    if (!deleteTarget) return;
    if (deleteTarget.type === 'project') deleteProject.mutate(deleteTarget.id);
    if (deleteTarget.type === 'blog') deleteBlog.mutate(deleteTarget.id);
  }

  if (!session) {
    return (
      <section className={styles.wrap} aria-labelledby="godmode-login-heading">
        {toast && (
          <div className={`${styles.toast} ${styles[toast.type]}`} role="status" aria-live="polite">
            {toast.message}
          </div>
        )}
        <div className={styles.loginShell}>
          <form
            className={`${styles.loginPanel} glass-card`}
            onSubmit={loginForm.handleSubmit((values) => loginMutation.mutate(values))}
            noValidate
          >
            <div className={styles.loginIcon} aria-hidden="true">
              <LogIn size={24} />
            </div>
            <div className="section-eyebrow">Godmode</div>
            <h2 id="godmode-login-heading" className={styles.loginTitle}>Admin login</h2>

            <label className={styles.label} htmlFor="admin-email">Email</label>
            <input
              id="admin-email"
              className={`${styles.input} ${loginForm.formState.errors.email ? styles.inputErr : ''}`}
              autoComplete="username"
              {...loginForm.register('email')}
            />
            {loginForm.formState.errors.email && (
              <span className={styles.errMsg}>{loginForm.formState.errors.email.message}</span>
            )}

            <label className={styles.label} htmlFor="admin-password">Password</label>
            <input
              id="admin-password"
              type="password"
              className={`${styles.input} ${loginForm.formState.errors.password ? styles.inputErr : ''}`}
              autoComplete="current-password"
              {...loginForm.register('password')}
            />
            {loginForm.formState.errors.password && (
              <span className={styles.errMsg}>{loginForm.formState.errors.password.message}</span>
            )}

            <button type="submit" className={`btn-primary ${styles.loginButton}`} disabled={loginMutation.isPending}>
              <LogIn size={16} />
              {loginMutation.isPending ? 'Signing in' : 'Sign in'}
            </button>
          </form>
        </div>
      </section>
    );
  }

  const projectPending = createProject.isPending || updateProject.isPending;
  const blogPending = createBlog.isPending || updateBlog.isPending;

  return (
    <section className={styles.wrap} aria-labelledby="godmode-heading">
      {toast && (
        <div className={`${styles.toast} ${styles[toast.type]}`} role="status" aria-live="polite">
          {toast.message}
        </div>
      )}

      <header className={styles.header}>
        <div>
          <div className="section-eyebrow">Godmode</div>
          <h2 id="godmode-heading" className={styles.title}>Admin workspace</h2>
        </div>
        <div className={styles.headerRight}>
          <span className={styles.emailBadge}>{session.email}</span>
          <button className={styles.iconTextButton} type="button" onClick={logout}>
            <LogOut size={16} />
            Log out
          </button>
        </div>
      </header>

      <div className={styles.tabs} role="tablist" aria-label="Admin sections">
        <button
          type="button"
          role="tab"
          aria-selected={activeTab === 'projects'}
          className={`${styles.tab} ${activeTab === 'projects' ? styles.activeTab : ''}`}
          onClick={() => setActiveTab('projects')}
        >
          <FolderKanban size={16} />
          Projects
        </button>
        <button
          type="button"
          role="tab"
          aria-selected={activeTab === 'blog'}
          className={`${styles.tab} ${activeTab === 'blog' ? styles.activeTab : ''}`}
          onClick={() => setActiveTab('blog')}
        >
          <Newspaper size={16} />
          Blog
        </button>
      </div>

      <div className={styles.layout}>
        <div className={styles.listPane}>
          <div className={styles.searchBar}>
            <Search size={16} aria-hidden="true" />
            <input
              className={styles.searchInput}
              placeholder={activeTab === 'projects' ? 'Search projects' : 'Search posts'}
              value={search}
              onChange={(event) => setSearch(event.target.value)}
            />
            <button
              type="button"
              className={styles.iconButton}
              onClick={() => {
                void projectsQuery.refetch();
                void blogQuery.refetch();
              }}
              aria-label="Refresh content"
            >
              <RefreshCw size={16} />
            </button>
          </div>

          {activeTab === 'projects' ? (
            <div className={styles.listBody}>
              <div className={styles.listSummary}>
                <span>{projects.length} projects</span>
                <button type="button" className={styles.iconTextButton} onClick={resetProjectForm}>
                  <Plus size={16} />
                  New
                </button>
              </div>
              {projectsQuery.isLoading ? (
                <p className={styles.empty}>Loading projects</p>
              ) : filteredProjects.length === 0 ? (
                <p className={styles.empty}>No projects found</p>
              ) : (
                <ul className={styles.list} role="list">
                  {filteredProjects.map((project) => (
                    <li key={project.id} className={`${styles.listItem} ${editingProjectId === project.id ? styles.editing : ''}`}>
                      <div className={styles.listInfo}>
                        <p className={styles.listTitle}>{project.title}</p>
                        <p className={styles.listMeta}>
                          {project.categories.map((category) => CATEGORY_LABEL[category]).join(' / ')}
                        </p>
                      </div>
                      <div className={styles.listActions}>
                        <button
                          type="button"
                          className={styles.iconButton}
                          onClick={() => startProjectEdit(project)}
                          aria-label={`Edit ${project.title}`}
                        >
                          <Pencil size={15} />
                        </button>
                        <button
                          type="button"
                          className={`${styles.iconButton} ${styles.dangerButton}`}
                          onClick={() => setDeleteTarget({ type: 'project', id: project.id })}
                          aria-label={`Delete ${project.title}`}
                        >
                          <Trash2 size={15} />
                        </button>
                      </div>
                    </li>
                  ))}
                </ul>
              )}
            </div>
          ) : (
            <div className={styles.listBody}>
              <div className={styles.listSummary}>
                <span>{posts.length} posts</span>
                <button type="button" className={styles.iconTextButton} onClick={resetBlogForm}>
                  <Plus size={16} />
                  New
                </button>
              </div>
              {blogQuery.isLoading ? (
                <p className={styles.empty}>Loading posts</p>
              ) : filteredPosts.length === 0 ? (
                <p className={styles.empty}>No posts found</p>
              ) : (
                <ul className={styles.list} role="list">
                  {filteredPosts.map((post) => (
                    <li key={post.id} className={`${styles.listItem} ${editingBlogId === post.id ? styles.editing : ''}`}>
                      <div className={styles.listInfo}>
                        <p className={styles.listTitle}>{post.title}</p>
                        <p className={styles.listMeta}>
                          {new Date(post.publishedAt).toLocaleDateString()} / {post.isPublished ? 'Published' : 'Draft'}
                        </p>
                      </div>
                      <div className={styles.listActions}>
                        <button
                          type="button"
                          className={styles.iconButton}
                          onClick={() => startBlogEdit(post)}
                          aria-label={`Edit ${post.title}`}
                        >
                          <Pencil size={15} />
                        </button>
                        <button
                          type="button"
                          className={`${styles.iconButton} ${styles.dangerButton}`}
                          onClick={() => setDeleteTarget({ type: 'blog', id: post.id })}
                          aria-label={`Delete ${post.title}`}
                        >
                          <Trash2 size={15} />
                        </button>
                      </div>
                    </li>
                  ))}
                </ul>
              )}
            </div>
          )}
        </div>

        <div className={`${styles.formPane} glass-card`}>
          {activeTab === 'projects' ? (
            <form onSubmit={projectForm.handleSubmit(submitProject)} noValidate>
              <div className={styles.formHeader}>
                <span className={styles.formMode}>{editingProjectId ? 'Editing project' : 'New project'}</span>
                <h3 className={styles.formTitle}>{editingProjectId ? 'Update project' : 'Create project'}</h3>
              </div>

              <div className={styles.field}>
                <label className={styles.label} htmlFor="project-title">Title</label>
                <input id="project-title" className={styles.input} {...projectForm.register('title')} />
                {projectForm.formState.errors.title && <span className={styles.errMsg}>{projectForm.formState.errors.title.message}</span>}
              </div>

              <div className={styles.field}>
                <label className={styles.label} htmlFor="project-description">Description</label>
                <textarea id="project-description" rows={4} className={`${styles.input} ${styles.textarea}`} {...projectForm.register('description')} />
                {projectForm.formState.errors.description && <span className={styles.errMsg}>{projectForm.formState.errors.description.message}</span>}
              </div>

              <div className={styles.fieldRow}>
                <div className={styles.field}>
                  <label className={styles.label} htmlFor="project-image">Image URL</label>
                  <input id="project-image" className={styles.input} {...projectForm.register('imageUrl')} />
                  {projectForm.formState.errors.imageUrl && <span className={styles.errMsg}>{projectForm.formState.errors.imageUrl.message}</span>}
                </div>
                <div className={styles.field}>
                  <label className={styles.label} htmlFor="project-order">Order</label>
                  <input id="project-order" type="number" min={0} className={styles.input} {...projectForm.register('displayOrder')} />
                </div>
              </div>

              <div className={styles.field}>
                <label className={styles.label} htmlFor="project-tech">Technologies</label>
                <input id="project-tech" className={styles.input} placeholder="React, Node.js, PostgreSQL" {...projectForm.register('technologies')} />
                {projectForm.formState.errors.technologies && <span className={styles.errMsg}>{projectForm.formState.errors.technologies.message}</span>}
              </div>

              <div className={styles.field}>
                <label className={styles.label} htmlFor="project-url">Live URL</label>
                <input id="project-url" className={styles.input} placeholder="https://example.com" {...projectForm.register('liveUrl')} />
                {projectForm.formState.errors.liveUrl && <span className={styles.errMsg}>{projectForm.formState.errors.liveUrl.message}</span>}
              </div>

              <div className={styles.field}>
                <span className={styles.label}>Categories</span>
                <div className={styles.optionGrid}>
                  {CATEGORIES.map((category) => (
                    <label key={category.value} className={styles.checkOption}>
                      <input type="checkbox" value={category.value} {...projectForm.register('categories')} />
                      <span>{category.label}</span>
                    </label>
                  ))}
                </div>
                {projectForm.formState.errors.categories && <span className={styles.errMsg}>{projectForm.formState.errors.categories.message}</span>}
              </div>

              <div className={styles.formActions}>
                <button type="button" className={styles.iconTextButton} onClick={resetProjectForm}>
                  <X size={16} />
                  Cancel
                </button>
                <button type="submit" className={`btn-primary ${styles.saveButton}`} disabled={projectPending}>
                  <Save size={16} />
                  {projectPending ? 'Saving' : 'Save'}
                </button>
              </div>
            </form>
          ) : (
            <form onSubmit={blogForm.handleSubmit(submitBlog)} noValidate>
              <div className={styles.formHeader}>
                <span className={styles.formMode}>{editingBlogId ? 'Editing post' : 'New post'}</span>
                <h3 className={styles.formTitle}>{editingBlogId ? 'Update blog post' : 'Create blog post'}</h3>
              </div>

              <div className={styles.field}>
                <label className={styles.label} htmlFor="blog-title">Title</label>
                <input id="blog-title" className={styles.input} {...blogForm.register('title')} />
                {blogForm.formState.errors.title && <span className={styles.errMsg}>{blogForm.formState.errors.title.message}</span>}
              </div>

              <div className={styles.field}>
                <label className={styles.label} htmlFor="blog-excerpt">Excerpt</label>
                <textarea id="blog-excerpt" rows={3} className={`${styles.input} ${styles.textarea}`} {...blogForm.register('excerpt')} />
                {blogForm.formState.errors.excerpt && <span className={styles.errMsg}>{blogForm.formState.errors.excerpt.message}</span>}
              </div>

              <div className={styles.field}>
                <label className={styles.label} htmlFor="blog-content">Content</label>
                <textarea id="blog-content" rows={6} className={`${styles.input} ${styles.textarea}`} {...blogForm.register('content')} />
              </div>

              <div className={styles.fieldRow}>
                <div className={styles.field}>
                  <label className={styles.label} htmlFor="blog-image">Image URL</label>
                  <input id="blog-image" className={styles.input} {...blogForm.register('imageUrl')} />
                  {blogForm.formState.errors.imageUrl && <span className={styles.errMsg}>{blogForm.formState.errors.imageUrl.message}</span>}
                </div>
                <div className={styles.field}>
                  <label className={styles.label} htmlFor="blog-date">Publish date</label>
                  <input id="blog-date" type="datetime-local" className={styles.input} {...blogForm.register('publishedAt')} />
                  {blogForm.formState.errors.publishedAt && <span className={styles.errMsg}>{blogForm.formState.errors.publishedAt.message}</span>}
                </div>
              </div>

              <div className={styles.fieldRow}>
                <div className={styles.field}>
                  <label className={styles.label} htmlFor="blog-author">Author</label>
                  <input id="blog-author" className={styles.input} {...blogForm.register('author')} />
                  {blogForm.formState.errors.author && <span className={styles.errMsg}>{blogForm.formState.errors.author.message}</span>}
                </div>
                <div className={styles.field}>
                  <label className={styles.label} htmlFor="blog-tags">Tags</label>
                  <input id="blog-tags" className={styles.input} placeholder="Career, React" {...blogForm.register('tags')} />
                </div>
              </div>

              <label className={styles.toggleRow}>
                <input type="checkbox" {...blogForm.register('isPublished')} />
                <span>Published</span>
              </label>

              <div className={styles.formActions}>
                <button type="button" className={styles.iconTextButton} onClick={resetBlogForm}>
                  <X size={16} />
                  Cancel
                </button>
                <button type="submit" className={`btn-primary ${styles.saveButton}`} disabled={blogPending}>
                  <Save size={16} />
                  {blogPending ? 'Saving' : 'Save'}
                </button>
              </div>
            </form>
          )}
        </div>
      </div>

      {deleteTarget && (
        <div className={styles.confirmOverlay} role="dialog" aria-modal="true" aria-labelledby="delete-title">
          <div className={`${styles.confirmBox} glass-card`}>
            <h3 id="delete-title">Confirm delete</h3>
            <p>This cannot be undone.</p>
            <div className={styles.confirmActions}>
              <button type="button" className={styles.iconTextButton} onClick={() => setDeleteTarget(null)}>
                <X size={16} />
                Cancel
              </button>
              <button type="button" className={`${styles.iconTextButton} ${styles.deleteConfirm}`} onClick={confirmDelete}>
                <Check size={16} />
                Delete
              </button>
            </div>
          </div>
        </div>
      )}
    </section>
  );
}
