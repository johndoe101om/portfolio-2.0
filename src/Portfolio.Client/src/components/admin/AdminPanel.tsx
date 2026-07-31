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
  Upload,
  Copy,
  Eye,
  Star,
  Archive,
  RotateCcw,
  LayoutGrid,
  List,
  Layers,
  Sparkles,
  Link as LinkIcon,
  CheckCircle,
  FileText,
  Clock,
  Shield,
  UploadCloud,
} from 'lucide-react';
import { apiClient } from '../../api/client';
import { PROJECTS, BLOG_POSTS } from '../../api/staticData';
import type {
  BlogPost,
  Project,
  ProjectImage,
  ProjectLink,
  ProjectFeature,
  ProjectAchievement,
  ProjectDashboardStats,
  AuditLog,
} from '../../types';
import styles from './AdminPanel.module.css';

const SESSION_KEY = 'portfolio_admin_session';

type AdminSession = {
  token: string;
  email: string;
  expiresAt: string;
};

type Toast = { message: string; type: 'success' | 'error' | 'info' };

const loginSchema = z.object({
  email: z.string().email('Enter a valid admin email'),
  password: z.string().min(1, 'Enter password'),
});

type LoginValues = z.infer<typeof loginSchema>;

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

export function AdminPanel() {
  const queryClient = useQueryClient();
  const [session, setSession] = useState<AdminSession | null>(() => readSession());
  const [activeTab, setActiveTab] = useState<'projects' | 'blog'>('projects');
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');

  // Filters & Search
  const [searchQuery, setSearchQuery] = useState('');
  const [categoryFilter, setCategoryFilter] = useState('*');
  const [statusFilter, setStatusFilter] = useState('');
  const [techFilter, setTechFilter] = useState('');
  const [sortBy, setSortBy] = useState('Newest');

  // Selection for Bulk Actions
  const [selectedIds, setSelectedIds] = useState<number[]>([]);

  // Modals
  const [isProjectModalOpen, setIsProjectModalOpen] = useState(true);
  const [editingProject, setEditingProject] = useState<Project | null>(null);
  const [modalTab, setModalTab] = useState<'basic' | 'media' | 'timeline' | 'tech' | 'links' | 'features' | 'achievements' | 'readme' | 'seo'>('basic');
  const [previewProject, setPreviewProject] = useState<Project | null>(null);
  const [deleteConfirmTarget, setDeleteConfirmTarget] = useState<number | null>(null);
  const [loginError, setLoginError] = useState('');

  // Toast
  const [toast, setToast] = useState<Toast | null>(null);

  const showToast = (message: string, type: Toast['type'] = 'success') => {
    setToast({ message, type });
    setTimeout(() => setToast(null), 4000);
  };

  // Form State for Project Mutation
  const [formData, setFormData] = useState({
    title: '',
    slug: '',
    shortDescription: '',
    fullDescription: '',
    status: 'Completed',
    visibility: 'Public',
    isPublished: true,
    isFeatured: false,
    resumeCategory: 'Web',
    experienceType: 'Professional',
    startDate: '',
    endDate: '',
    isCurrentlyWorking: false,
    readmeMarkdown: '# Project Overview\n\nDetailed project documentation.',
    metaTitle: '',
    metaDescription: '',
    metaKeywords: '',
    ogImageUrl: '',
    displayOrder: 0,
    thumbnailUrl: '/assets/images/placeholder.png',
    categories: [] as string[],
    technologies: [] as string[],
    technologiesText: '',
    skills: [] as string[],
    images: [] as ProjectImage[],
    links: [] as ProjectLink[],
    features: [] as ProjectFeature[],
    achievements: [] as ProjectAchievement[],
  });

  const authHeaders = useMemo(() => {
    return session?.token ? { Authorization: `Bearer ${session.token}` } : {};
  }, [session]);

  // Query Dashboard Stats
  const { data: stats } = useQuery<ProjectDashboardStats>({
    queryKey: ['admin-project-stats'],
    queryFn: async () => {
      if (!session?.token) {
        return {
          totalProjects: PROJECTS.length,
          publishedProjects: PROJECTS.length,
          draftProjects: 0,
          featuredProjects: 2,
          archivedProjects: 0,
          totalTechnologies: 12,
          totalCategories: 4,
          recentProjects: PROJECTS as unknown as Project[],
        };
      }
      try {
        const res = await apiClient.get('/api/projects/dashboard-stats', { headers: authHeaders });
        return res.data;
      } catch {
        return null;
      }
    },
    enabled: Boolean(session?.token),
  });

  // Query Projects
  const { data: projectsData, isLoading: projectsLoading, refetch: refetchProjects } = useQuery({
    queryKey: ['admin-projects', searchQuery, categoryFilter, statusFilter, techFilter, sortBy],
    queryFn: async () => {
      if (!session?.token) return PROJECTS as unknown as Project[];
      try {
        const res = await apiClient.get('/api/projects/paged', {
          headers: authHeaders,
          params: {
            search: searchQuery || undefined,
            category: categoryFilter !== '*' ? categoryFilter : undefined,
            status: statusFilter || undefined,
            technology: techFilter || undefined,
            sortBy,
            includeUnpublished: true,
            includeDeleted: false,
            page: 1,
            pageSize: 100,
          },
        });
        return (res.data?.items ?? res.data ?? []) as Project[];
      } catch {
        try {
          const fallbackRes = await apiClient.get('/api/projects', { headers: authHeaders });
          return (fallbackRes.data?.items ?? fallbackRes.data ?? []) as Project[];
        } catch {
          return PROJECTS as unknown as Project[];
        }
      }
    },
    enabled: Boolean(session?.token),
  });

  const projectsList = (projectsData ?? []) as Project[];

  // Query Blog Posts
  const { data: blogPostsData } = useQuery({
    queryKey: ['admin-blog-posts'],
    queryFn: async () => {
      if (!session?.token) return BLOG_POSTS as unknown as BlogPost[];
      try {
        const res = await apiClient.get('/api/blog', { headers: authHeaders });
        return (res.data ?? BLOG_POSTS) as BlogPost[];
      } catch {
        return BLOG_POSTS as unknown as BlogPost[];
      }
    },
    enabled: Boolean(session?.token),
  });
  const blogList = (blogPostsData ?? BLOG_POSTS) as BlogPost[];

  // Login Handler
  const loginForm = useForm<LoginValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: '', password: '' },
  });

  const loginMutation = useMutation({
    mutationFn: async (val: LoginValues) => {
      const res = await apiClient.post('/api/godmode/login', val);
      return res.data;
    },
    onSuccess: (data) => {
      const newSession: AdminSession = {
        token: data.token,
        email: data.email,
        expiresAt: data.expiresAt,
      };
      window.localStorage.setItem(SESSION_KEY, JSON.stringify(newSession));
      setSession(newSession);
      showToast('Welcome back, Admin!');
    },
    onError: (err: Error) => {
      showToast(err.message || 'Login failed', 'error');
    },
  });

  const handleLogout = () => {
    window.localStorage.removeItem(SESSION_KEY);
    setSession(null);
    showToast('Logged out successfully', 'info');
  };

  // Open Create Modal
  const handleOpenCreate = () => {
    setEditingProject(null);
    setFormData({
      title: '',
      slug: '',
      shortDescription: '',
      fullDescription: '',
      status: 'Completed',
      visibility: 'Public',
      isPublished: true,
      isFeatured: false,
      resumeCategory: 'Web',
      experienceType: 'Professional',
      startDate: '',
      endDate: '',
      isCurrentlyWorking: false,
      readmeMarkdown: '# Project README\n\nSystem documentation and setup instructions.',
      metaTitle: '',
      metaDescription: '',
      metaKeywords: '',
      ogImageUrl: '',
      displayOrder: 0,
      thumbnailUrl: '/assets/images/placeholder.png',
      categories: ['Web Design'],
      technologies: ['React', 'C#'],
      technologiesText: 'React, C#',
      skills: [],
      images: [],
      links: [{ id: 1, linkType: 'Live', url: '', label: 'Live Demo' }],
      features: [{ id: 1, title: 'Responsive UI', description: 'Supports dark mode & glassmorphism', displayOrder: 1 }],
      achievements: [{ id: 1, title: 'Launched Production', description: 'Deployed successfully', displayOrder: 1 }],
    });
    setModalTab('basic');
    setIsProjectModalOpen(true);
  };

  // Open Edit Modal
  const handleOpenEdit = (p: Project) => {
    setEditingProject(p);
    setFormData({
      title: p.title || '',
      slug: p.slug || '',
      shortDescription: p.shortDescription || p.description || '',
      fullDescription: p.fullDescription || '',
      status: p.status || 'Completed',
      visibility: p.visibility || 'Public',
      isPublished: p.isPublished ?? true,
      isFeatured: p.isFeatured ?? false,
      resumeCategory: p.resumeCategory || 'Web',
      experienceType: p.experienceType || 'Professional',
      startDate: p.startDate ? p.startDate.split('T')[0] : '',
      endDate: p.endDate ? p.endDate.split('T')[0] : '',
      isCurrentlyWorking: p.isCurrentlyWorking ?? false,
      readmeMarkdown: p.readmeMarkdown || '# README',
      metaTitle: p.metaTitle || '',
      metaDescription: p.metaDescription || '',
      metaKeywords: p.metaKeywords || '',
      ogImageUrl: p.ogImageUrl || '',
      displayOrder: p.displayOrder || 0,
      thumbnailUrl: p.thumbnailUrl || p.imageUrl || '/assets/images/placeholder.png',
      categories: p.categories || ['Web Design'],
      technologies: p.technologies || [],
      technologiesText: (p.technologies || []).join(', '),
      skills: (p.skills || []) as string[],
      images: p.images || [],
      links: p.links || [],
      features: p.features || [],
      achievements: p.achievements || [],
    });
    setModalTab('basic');
    setIsProjectModalOpen(true);
  };

  // Save Project Mutation
  const saveProjectMutation = useMutation({
    mutationFn: async () => {
      const payload = {
        title: formData.title,
        slug: formData.slug || undefined,
        shortDescription: formData.shortDescription || formData.fullDescription,
        fullDescription: formData.fullDescription || formData.shortDescription,
        description: formData.fullDescription || formData.shortDescription,
        status: formData.status,
        visibility: formData.visibility,
        isPublished: formData.isPublished,
        isFeatured: formData.isFeatured,
        resumeCategory: formData.resumeCategory,
        experienceType: formData.experienceType,
        startDate: formData.startDate ? new Date(formData.startDate).toISOString() : null,
        endDate: formData.endDate ? new Date(formData.endDate).toISOString() : null,
        isCurrentlyWorking: formData.isCurrentlyWorking,
        readmeMarkdown: formData.readmeMarkdown,
        metaTitle: formData.metaTitle,
        metaDescription: formData.metaDescription,
        metaKeywords: formData.metaKeywords,
        ogImageUrl: formData.ogImageUrl,
        displayOrder: Number(formData.displayOrder),
        thumbnailUrl: formData.thumbnailUrl,
        categories: formData.categories.map((c) => c === 'Web Design' ? 'webdesign' : c === 'Web App' ? 'webapp' : c === 'Mobile' ? 'mobiledesign' : c === 'Game' ? 'gamedesign' : c.toLowerCase()),
        technologies: (formData.technologiesText ? formData.technologiesText.split(',').map((t) => t.trim()).filter(Boolean) : formData.technologies),
        skills: formData.skills,
        images: formData.images,
        links: formData.links,
        features: formData.features,
        achievements: formData.achievements,
      };

      if (editingProject) {
        const res = await apiClient.put(`/api/projects/${editingProject.id}`, payload, { headers: authHeaders });
        return res.data;
      } else {
        const res = await apiClient.post('/api/projects', payload, { headers: authHeaders });
        return res.data;
      }
    },
    onSuccess: () => {
      showToast(editingProject ? 'Project updated successfully!' : 'Project created successfully!');
      setIsProjectModalOpen(false);
      queryClient.invalidateQueries({ queryKey: ['admin-projects'] });
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      queryClient.invalidateQueries({ queryKey: ['admin-project-stats'] });
    },
    onError: (err: Error) => {
      showToast(err.message || 'Failed to save project', 'error');
    },
  });

  // Upload Thumbnail Image to Supabase
  const handleUploadThumbnail = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    const data = new FormData();
    data.append('file', file);

    try {
      showToast('Uploading thumbnail to Supabase Storage...', 'info');
      const res = await apiClient.post('/api/projects/upload-image?folder=thumbnails', data, {
        headers: { ...authHeaders, 'Content-Type': 'multipart/form-data' },
      });

      if (res.data?.publicUrl) {
        setFormData((prev) => ({ ...prev, thumbnailUrl: res.data.publicUrl }));
        showToast('Thumbnail uploaded successfully!');
      }
    } catch (err: unknown) {
      const errorMsg = err instanceof Error ? err.message : 'Upload failed';
      showToast(errorMsg, 'error');
    }
  };

  // Upload Gallery Image
  const handleUploadGalleryImage = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (!files || files.length === 0) return;

    for (let i = 0; i < files.length; i++) {
      const data = new FormData();
      data.append('file', files[i]);
      try {
        const res = await apiClient.post('/api/projects/upload-image?folder=gallery', data, {
          headers: { ...authHeaders, 'Content-Type': 'multipart/form-data' },
        });

        if (res.data?.publicUrl) {
          const newImg: ProjectImage = {
            id: Date.now() + i,
            storagePath: res.data.storagePath,
            publicUrl: res.data.publicUrl,
            altText: files[i].name,
            isThumbnail: false,
            displayOrder: formData.images.length + 1,
          };
          setFormData((prev) => ({ ...prev, images: [...prev.images, newImg] }));
        }
      } catch (err: unknown) {
        const errorMsg = err instanceof Error ? err.message : 'Upload failed';
        showToast(errorMsg, 'error');
      }
    }
    showToast('Gallery images uploaded!');
  };

  // Single Action Handlers
  const handleDuplicate = async (id: number) => {
    try {
      await apiClient.post(`/api/projects/${id}/duplicate`, {}, { headers: authHeaders });
      showToast('Project duplicated successfully!');
      queryClient.invalidateQueries({ queryKey: ['admin-projects'] });
      queryClient.invalidateQueries({ queryKey: ['projects'] });
    } catch (err: unknown) {
      const errorMsg = err instanceof Error ? err.message : 'Duplicate failed';
      showToast(errorMsg, 'error');
    }
  };

  const handleTogglePublish = async (id: number, current: boolean) => {
    try {
      await apiClient.post(`/api/projects/${id}/publish?publish=${!current}`, {}, { headers: authHeaders });
      showToast(current ? 'Project unpublished' : 'Project published!');
      queryClient.invalidateQueries({ queryKey: ['admin-projects'] });
      queryClient.invalidateQueries({ queryKey: ['projects'] });
    } catch (err: unknown) {
      const errorMsg = err instanceof Error ? err.message : 'Action failed';
      showToast(errorMsg, 'error');
    }
  };

  const handleToggleFeature = async (id: number) => {
    try {
      await apiClient.post(`/api/projects/${id}/feature`, {}, { headers: authHeaders });
      showToast('Featured status updated!');
      queryClient.invalidateQueries({ queryKey: ['admin-projects'] });
      queryClient.invalidateQueries({ queryKey: ['projects'] });
    } catch (err: unknown) {
      const errorMsg = err instanceof Error ? err.message : 'Action failed';
      showToast(errorMsg, 'error');
    }
  };

  const handleDelete = async (id: number) => {
    try {
      await apiClient.delete(`/api/projects/${id}`, { headers: authHeaders });
      showToast('Project deleted');
      queryClient.invalidateQueries({ queryKey: ['admin-projects'] });
      queryClient.invalidateQueries({ queryKey: ['projects'] });
    } catch (err: unknown) {
      const errorMsg = err instanceof Error ? err.message : 'Delete failed';
      showToast(errorMsg, 'error');
    }
  };

  // Bulk Actions
  const handleBulkAction = async (action: string) => {
    if (selectedIds.length === 0) return;
    try {
      await apiClient.post('/api/projects/bulk-action', { projectIds: selectedIds, action }, { headers: authHeaders });
      showToast(`Bulk '${action}' applied to ${selectedIds.length} projects!`);
      setSelectedIds([]);
      queryClient.invalidateQueries({ queryKey: ['admin-projects'] });
      queryClient.invalidateQueries({ queryKey: ['projects'] });
    } catch (err: unknown) {
      const errorMsg = err instanceof Error ? err.message : 'Bulk action failed';
      showToast(errorMsg, 'error');
    }
  };

  // LOGIN SCREEN
  if (!session) {
    return (
      <div className={styles.adminContainer}>
        <div className={styles.loginCard}>
          <div className={styles.titleGroup} style={{ textAlign: 'center', marginBottom: 24 }}>
            <Shield size={32} color="#0284c7" style={{ margin: '0 auto 8px' }} />
            <h1>Admin Login & Workspace Access</h1>
            <p>Access the Portfolio Project Management Suite</p>
          </div>

          <form onSubmit={(e) => {
            e.preventDefault();
            const val = loginForm.getValues();
            if (!val.password) {
              setLoginError('Enter the admin password');
              return;
            }
            setLoginError('');
            loginMutation.mutate({ email: val.email || 'admin@codersatyam.com', password: val.password });
          }}>
            <div className={styles.fieldGroup}>
              <label htmlFor="admin-email">Email Address</label>
              <input
                {...loginForm.register('email')}
                id="admin-email"
                type="email"
                placeholder="admin@codersatyam.com"
                className={styles.fieldInput}
              />
            </div>
            <div className={styles.fieldGroup}>
              <label htmlFor="admin-password">Password</label>
              <input
                {...loginForm.register('password')}
                id="admin-password"
                type="password"
                placeholder="••••••••"
                className={styles.fieldInput}
              />
              {loginError && <p style={{ color: '#ef4444', fontSize: 12, marginTop: 4 }}>{loginError}</p>}
            </div>

            <button
              type="submit"
              disabled={loginMutation.isPending}
              className={styles.btnPrimary}
              style={{ width: '100%', justifyContent: 'center', marginTop: 16 }}
            >
              <LogIn size={16} /> Sign in
            </button>
          </form>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.adminContainer}>
      {/* Toast Notification */}
      {toast && (
        <div className={`${styles.toast} ${toast.type === 'error' ? styles.toastError : toast.type === 'info' ? styles.toastInfo : styles.toastSuccess}`}>
          <CheckCircle size={16} /> {toast.message}
        </div>
      )}

      {/* Header Row */}
      <div className={styles.headerRow}>
        <div className={styles.titleGroup}>
          <h1>
            <FolderKanban color="#38bdf8" /> Admin Workspace & Project Management
          </h1>
          <p>Logged in as {session.email} • <span>{projectsList.length} projects</span></p>
        </div>

        <div style={{ display: 'flex', gap: 10 }}>
          <button className={styles.btnSecondary} onClick={() => refetchProjects()}>
            <RefreshCw size={14} /> Refresh
          </button>
          <button className={styles.btnPrimary} onClick={handleOpenCreate}>
            <Plus size={16} /> Create Project
          </button>
          <button className={styles.btnSecondary} onClick={handleLogout}>
            <LogOut size={14} /> Logout
          </button>
        </div>
      </div>

      {/* Main Module Nav Tabs */}
      <div role="tablist" style={{ display: 'flex', gap: 8, marginBottom: 20 }}>
        <button
          role="tab"
          aria-selected={activeTab === 'projects'}
          className={`${styles.tabBtn} ${activeTab === 'projects' ? styles.tabBtnActive : ''}`}
          onClick={() => setActiveTab('projects')}
        >
          <FolderKanban size={14} style={{ display: 'inline', marginRight: 4 }} /> Projects ({projectsList.length})
        </button>
        <button
          role="tab"
          aria-selected={activeTab === 'blog'}
          className={`${styles.tabBtn} ${activeTab === 'blog' ? styles.tabBtnActive : ''}`}
          onClick={() => setActiveTab('blog')}
        >
          <Newspaper size={14} style={{ display: 'inline', marginRight: 4 }} /> Blog (1 posts)
        </button>
      </div>

      {/* Metrics Row */}
      {stats && (
        <div className={styles.statsRow}>
          <div className={styles.statCard}>
            <div className={styles.statVal}>{stats.totalProjects}</div>
            <div className={styles.statLbl}>Total Projects</div>
          </div>
          <div className={styles.statCard}>
            <div className={styles.statVal} style={{ color: '#4ade80' }}>{stats.publishedProjects}</div>
            <div className={styles.statLbl}>Published</div>
          </div>
          <div className={styles.statCard}>
            <div className={styles.statVal} style={{ color: '#facc15' }}>{stats.draftProjects}</div>
            <div className={styles.statLbl}>Drafts</div>
          </div>
          <div className={styles.statCard}>
            <div className={styles.statVal} style={{ color: '#f472b6' }}>{stats.featuredProjects}</div>
            <div className={styles.statLbl}>Featured</div>
          </div>
          <div className={styles.statCard}>
            <div className={styles.statVal} style={{ color: '#a78bfa' }}>{stats.totalTechnologies}</div>
            <div className={styles.statLbl}>Technologies</div>
          </div>
        </div>
      )}

      {/* Toolbar */}
      <div className={styles.toolbar}>
        <div className={styles.searchGroup}>
          <Search size={16} color="#94a3b8" />
          <input
            type="text"
            placeholder="Search projects by title, tech, description, slug..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>

        <div className={styles.filterGroup}>
          <select className={styles.selectInput} value={categoryFilter} onChange={(e) => setCategoryFilter(e.target.value)}>
            <option value="*">All Categories</option>
            <option value="webdesign">Web Design</option>
            <option value="webapp">Web App</option>
            <option value="mobiledesign">Mobile</option>
            <option value="gamedesign">Game</option>
          </select>

          <select className={styles.selectInput} value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
            <option value="">All Statuses</option>
            <option value="Completed">Completed</option>
            <option value="Planning">Planning</option>
            <option value="Draft">Draft</option>
            <option value="Archived">Archived</option>
          </select>

          <select className={styles.selectInput} value={sortBy} onChange={(e) => setSortBy(e.target.value)}>
            <option value="Newest">Newest First</option>
            <option value="Oldest">Oldest First</option>
            <option value="Alphabetical">Alphabetical</option>
            <option value="Featured">Featured First</option>
          </select>

          <div style={{ display: 'flex', gap: 2 }}>
            <button
              className={styles.btnSecondary}
              style={{ padding: '8px 10px', background: viewMode === 'grid' ? '#0284c7' : undefined }}
              onClick={() => setViewMode('grid')}
            >
              <LayoutGrid size={14} />
            </button>
            <button
              className={styles.btnSecondary}
              style={{ padding: '8px 10px', background: viewMode === 'list' ? '#0284c7' : undefined }}
              onClick={() => setViewMode('list')}
            >
              <List size={14} />
            </button>
          </div>
        </div>
      </div>

      {/* Floating Bulk Bar */}
      {selectedIds.length > 0 && (
        <div className={styles.bulkBar}>
          <div>Selected <strong>{selectedIds.length}</strong> project(s)</div>
          <div className={styles.bulkActions}>
            <button className={styles.bulkBtn} onClick={() => handleBulkAction('publish')}>Publish</button>
            <button className={styles.bulkBtn} onClick={() => handleBulkAction('unpublish')}>Unpublish</button>
            <button className={styles.bulkBtn} onClick={() => handleBulkAction('archive')}>Archive</button>
            <button className={styles.bulkBtn} onClick={() => handleBulkAction('feature')}>Feature</button>
            <button className={styles.bulkBtn} onClick={() => handleBulkAction('delete')}>Delete</button>
          </div>
        </div>
      )}

      {/* Projects Grid */}
      <div className={styles.projectsGrid}>
        {projectsList.map((p) => (
          <div key={p.id} className={styles.projectCard}>
            <div className={styles.cardThumb}>
              <input
                type="checkbox"
                className={styles.cardCheck}
                checked={selectedIds.includes(p.id)}
                onChange={(e) => {
                  if (e.target.checked) setSelectedIds((ids) => [...ids, p.id]);
                  else setSelectedIds((ids) => ids.filter((id) => id !== p.id));
                }}
              />

              <img src={p.thumbnailUrl || p.imageUrl || '/assets/images/placeholder.png'} alt={p.title} />

              <div className={styles.cardBadges}>
                {p.isFeatured && <span className={styles.badge} style={{ background: 'rgba(234, 179, 8, 0.3)', color: '#fde047' }}><Star size={10} style={{ display: 'inline' }} /> Featured</span>}
                <span className={styles.badge}>{p.status || 'Completed'}</span>
              </div>
            </div>

            <div className={styles.cardContent}>
              <div className={styles.cardHeader}>
                <h3>{p.title}</h3>
              </div>
              <p className={styles.cardDesc}>{p.shortDescription || p.description}</p>

              <div className={styles.tagRow}>
                {p.categories?.map((c) => <span key={c} className={styles.tag}>{c}</span>)}
                {p.technologies?.slice(0, 3).map((t) => <span key={t} className={styles.tag}>{t}</span>)}
              </div>

              <div className={styles.cardActions}>
                <span style={{ fontSize: 12, color: '#64748b' }}>{p.durationText || 'Completed'}</span>

                <div className={styles.actionBtnGroup}>
                  <button className={styles.iconBtn} title="Preview" aria-label={`Preview ${p.title}`} onClick={() => setPreviewProject(p)}><Eye size={14} /></button>
                  <button className={styles.iconBtn} title="Duplicate" aria-label={`Duplicate ${p.title}`} onClick={() => handleDuplicate(p.id)}><Copy size={14} /></button>
                  <button className={styles.iconBtn} title="Toggle Feature" aria-label={`Feature ${p.title}`} onClick={() => handleToggleFeature(p.id)}><Star size={14} /></button>
                  <button className={styles.iconBtn} title="Edit" aria-label={`Edit ${p.title}`} onClick={() => handleOpenEdit(p)}><Pencil size={14} /></button>
                  <button className={`${styles.iconBtn} ${styles.iconBtnDanger}`} title="Delete" aria-label={`Delete ${p.title}`} onClick={() => setDeleteConfirmTarget(p.id)}><Trash2 size={14} /></button>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {activeTab === 'blog' && (
        <div style={{ padding: 24, background: '#0f172a', borderRadius: 16, border: '1px solid rgba(255,255,255,0.1)' }}>
          {blogList.map((post) => (
            <div key={post.id} style={{ marginBottom: 12 }}>
              <h3>{post.title}</h3>
              <p style={{ color: '#94a3b8', fontSize: 13 }}>{post.excerpt}</p>
            </div>
          ))}
          <p style={{ color: '#64748b', fontSize: 12, marginTop: 16 }}>{blogList.length} posts</p>
        </div>
      )}

      {/* CREATE / EDIT PROJECT MODAL */}
      {isProjectModalOpen && (
        <div className={styles.modalBackdrop} onClick={() => setIsProjectModalOpen(false)}>
          <div className={styles.modalBox} onClick={(e) => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h2 style={{ fontSize: 18, fontWeight: 700, color: '#fff' }}>
                {editingProject ? `Edit Project: ${editingProject.title}` : 'Create New Industry-Standard Project'}
              </h2>
              <button className={styles.iconBtn} onClick={() => setIsProjectModalOpen(false)}><X size={18} /></button>
            </div>

            {/* Modal Tabs */}
            <div className={styles.modalTabs}>
              <button className={`${styles.tabBtn} ${modalTab === 'basic' ? styles.tabBtnActive : ''}`} onClick={() => setModalTab('basic')}>Basic Info</button>
              <button className={`${styles.tabBtn} ${modalTab === 'media' ? styles.tabBtnActive : ''}`} onClick={() => setModalTab('media')}>Media & Supabase Storage</button>
              <button className={`${styles.tabBtn} ${modalTab === 'timeline' ? styles.tabBtnActive : ''}`} onClick={() => setModalTab('timeline')}>Timeline & Duration</button>
              <button className={`${styles.tabBtn} ${modalTab === 'tech' ? styles.tabBtnActive : ''}`} onClick={() => setModalTab('tech')}>Technologies & Stack</button>
              <button className={`${styles.tabBtn} ${modalTab === 'links' ? styles.tabBtnActive : ''}`} onClick={() => setModalTab('links')}>External Links</button>
              <button className={`${styles.tabBtn} ${modalTab === 'features' ? styles.tabBtnActive : ''}`} onClick={() => setModalTab('features')}>Features</button>
              <button className={`${styles.tabBtn} ${modalTab === 'achievements' ? styles.tabBtnActive : ''}`} onClick={() => setModalTab('achievements')}>Achievements</button>
              <button className={`${styles.tabBtn} ${modalTab === 'readme' ? styles.tabBtnActive : ''}`} onClick={() => setModalTab('readme')}>Markdown README</button>
              <button className={`${styles.tabBtn} ${modalTab === 'seo' ? styles.tabBtnActive : ''}`} onClick={() => setModalTab('seo')}>SEO Metadata</button>
            </div>

            <div className={styles.modalBody}>
              <div style={{ display: modalTab === 'basic' ? 'block' : 'none' }}>
                <div className={styles.formGrid}>
                  <div className={styles.fieldGroup}>
                    <label htmlFor="project-title">Title</label>
                    <input id="project-title" type="text" className={styles.fieldInput} value={formData.title} onChange={(e) => setFormData({ ...formData, title: e.target.value })} placeholder="e.g. FashionAI SaaS Platform" />
                  </div>

                  <div className={styles.fieldGroup}>
                    <label htmlFor="project-slug">Slug (Auto-generated)</label>
                    <input id="project-slug" type="text" className={styles.fieldInput} value={formData.slug} onChange={(e) => setFormData({ ...formData, slug: e.target.value })} placeholder="fashion-ai-saas-platform" />
                  </div>

                  <div className={styles.fieldGroup} style={{ gridColumn: '1 / -1' }}>
                    <label htmlFor="project-short-desc">Short Description</label>
                    <input id="project-short-desc" type="text" className={styles.fieldInput} value={formData.shortDescription} onChange={(e) => setFormData({ ...formData, shortDescription: e.target.value })} placeholder="Brief summary of the project" />
                  </div>

                  <div className={styles.fieldGroup} style={{ gridColumn: '1 / -1' }}>
                    <label htmlFor="project-description">Description</label>
                    <textarea id="project-description" className={styles.fieldInput} rows={4} value={formData.fullDescription} onChange={(e) => setFormData({ ...formData, fullDescription: e.target.value, shortDescription: formData.shortDescription || e.target.value })} placeholder="Comprehensive description of architecture and goals" />
                  </div>

                  <div className={styles.fieldGroup}>
                    <label>Status</label>
                    <select className={styles.fieldInput} value={formData.status} onChange={(e) => setFormData({ ...formData, status: e.target.value })}>
                      <option value="Completed">Completed</option>
                      <option value="Planning">Planning</option>
                      <option value="In Progress">In Progress</option>
                      <option value="Draft">Draft</option>
                      <option value="Archived">Archived</option>
                    </select>
                  </div>

                  <div className={styles.fieldGroup}>
                    <label>Resume Category</label>
                    <select className={styles.fieldInput} value={formData.resumeCategory} onChange={(e) => setFormData({ ...formData, resumeCategory: e.target.value })}>
                      <option value="Web">Web</option>
                      <option value="Mobile">Mobile</option>
                      <option value="AI/Cloud">AI / Cloud</option>
                      <option value="DevOps">DevOps</option>
                      <option value="Game">Game</option>
                    </select>
                  </div>
                </div>
              </div>

              <div style={{ display: modalTab === 'media' ? 'block' : 'none' }}>
                <div>
                  <div className={styles.fieldGroup} style={{ marginBottom: 24 }}>
                    <label>Thumbnail Image (Upload to Supabase Storage)</label>
                    <div style={{ display: 'flex', gap: 16, alignItems: 'center' }}>
                      <img src={formData.thumbnailUrl} alt="Thumbnail" style={{ width: 120, height: 80, objectFit: 'cover', borderRadius: 10, border: '1px solid rgba(255,255,255,0.15)' }} />
                      <label className={styles.btnPrimary} style={{ cursor: 'pointer' }}>
                        <UploadCloud size={16} /> Choose File
                        <input type="file" accept="image/*" style={{ display: 'none' }} onChange={handleUploadThumbnail} />
                      </label>
                    </div>
                  </div>

                  <div className={styles.fieldGroup}>
                    <label>Gallery Upload (Multiple Images)</label>
                    <label className={styles.btnSecondary} style={{ cursor: 'pointer', display: 'inline-flex', width: 'fit-content', marginBottom: 12 }}>
                      <Upload size={16} /> Select Gallery Images
                      <input type="file" multiple accept="image/*" style={{ display: 'none' }} onChange={handleUploadGalleryImage} />
                    </label>

                    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(120px, 1fr))', gap: 10 }}>
                      {formData.images.map((img, idx) => (
                        <div key={idx} style={{ position: 'relative' }}>
                          <img src={img.publicUrl} alt="" style={{ width: '100%', height: 80, objectFit: 'cover', borderRadius: 8 }} />
                          <button
                            className={styles.iconBtnDanger}
                            style={{ position: 'absolute', top: 4, right: 4, width: 22, height: 22, borderRadius: 4, background: '#ef4444' }}
                            onClick={() => setFormData({ ...formData, images: formData.images.filter((_, i) => i !== idx) })}
                          >
                            <X size={12} />
                          </button>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
              </div>

              <div style={{ display: modalTab === 'timeline' ? 'block' : 'none' }}>
                <div className={styles.formGrid}>
                  <div className={styles.fieldGroup}>
                    <label>Start Date</label>
                    <input type="date" className={styles.fieldInput} value={formData.startDate} onChange={(e) => setFormData({ ...formData, startDate: e.target.value })} />
                  </div>
                  <div className={styles.fieldGroup}>
                    <label>End Date</label>
                    <input type="date" className={styles.fieldInput} disabled={formData.isCurrentlyWorking} value={formData.endDate} onChange={(e) => setFormData({ ...formData, endDate: e.target.value })} />
                  </div>
                  <div className={styles.fieldGroup} style={{ gridColumn: '1 / -1', flexDirection: 'row', alignItems: 'center', gap: 10 }}>
                    <input type="checkbox" id="currWork" checked={formData.isCurrentlyWorking} onChange={(e) => setFormData({ ...formData, isCurrentlyWorking: e.target.checked })} />
                    <label htmlFor="currWork" style={{ cursor: 'pointer' }}>Currently Working On This Project</label>
                  </div>
                </div>
              </div>

              <div style={{ display: modalTab === 'basic' || modalTab === 'tech' ? 'block' : 'none' }}>
                <div>
                  <div className={styles.fieldGroup}>
                    <label htmlFor="project-technologies">Technologies</label>
                    <input id="project-technologies" type="text" className={styles.fieldInput} value={formData.technologiesText} onChange={(e) => setFormData({ ...formData, technologiesText: e.target.value })} placeholder="React, C#, PostgreSQL, Supabase" />
                  </div>
                  <div className={styles.fieldGroup}>
                    <label>Categories</label>
                    <div style={{ display: 'flex', gap: 16, flexWrap: 'wrap' }}>
                      {['Web Design', 'Web App', 'Mobile', 'Game'].map((cat) => (
                        <label key={cat} style={{ display: 'flex', alignItems: 'center', gap: 6, cursor: 'pointer', fontSize: 13 }}>
                          <input
                            type="checkbox"
                            checked={formData.categories.includes(cat)}
                            onChange={(e) => {
                              if (e.target.checked) setFormData({ ...formData, categories: [...formData.categories, cat] });
                              else setFormData({ ...formData, categories: formData.categories.filter((c) => c !== cat) });
                            }}
                          />
                          {cat}
                        </label>
                      ))}
                    </div>
                  </div>
                </div>
              </div>

              {modalTab === 'links' && (
                <div>
                  {formData.links.map((link, idx) => (
                    <div key={idx} style={{ display: 'flex', gap: 10, marginBottom: 10, alignItems: 'center' }}>
                      <select className={styles.selectInput} value={link.linkType} onChange={(e) => {
                        const newLinks = [...formData.links];
                        newLinks[idx].linkType = e.target.value;
                        setFormData({ ...formData, links: newLinks });
                      }}>
                        <option value="Live">Live Demo</option>
                        <option value="GitHub">GitHub</option>
                        <option value="GitLab">GitLab</option>
                        <option value="Documentation">Documentation</option>
                        <option value="Figma">Figma</option>
                      </select>

                      <input type="text" className={styles.fieldInput} style={{ flex: 1 }} value={link.url} onChange={(e) => {
                        const newLinks = [...formData.links];
                        newLinks[idx].url = e.target.value;
                        setFormData({ ...formData, links: newLinks });
                      }} placeholder="https://..." />

                      <button className={styles.iconBtnDanger} onClick={() => setFormData({ ...formData, links: formData.links.filter((_, i) => i !== idx) })}><Trash2 size={14} /></button>
                    </div>
                  ))}
                  <button className={styles.btnSecondary} onClick={() => setFormData({ ...formData, links: [...formData.links, { id: Date.now(), linkType: 'Live', url: '', label: '' }] })}><Plus size={14} /> Add Link</button>
                </div>
              )}

              {modalTab === 'readme' && (
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16, height: 360 }}>
                  <div className={styles.fieldGroup}>
                    <label>Markdown Source</label>
                    <textarea className={styles.fieldInput} style={{ height: '100%', fontFamily: 'monospace' }} value={formData.readmeMarkdown} onChange={(e) => setFormData({ ...formData, readmeMarkdown: e.target.value })} />
                  </div>
                  <div className={styles.fieldGroup}>
                    <label>Live HTML Preview</label>
                    <div style={{ background: '#020617', padding: 16, borderRadius: 10, border: '1px solid rgba(255,255,255,0.1)', overflowY: 'auto', height: '100%', fontSize: 13, color: '#cbd5e1' }}>
                      <pre style={{ whiteSpace: 'pre-wrap' }}>{formData.readmeMarkdown}</pre>
                    </div>
                  </div>
                </div>
              )}
            </div>

            <div className={styles.modalFooter}>
              <button className={styles.btnSecondary} onClick={() => setIsProjectModalOpen(false)}>Cancel</button>
              <button className={styles.btnPrimary} disabled={saveProjectMutation.isPending} onClick={() => saveProjectMutation.mutate()}>
                <Save size={16} /> Save
              </button>
            </div>
          </div>
        </div>
      )}
      {deleteConfirmTarget !== null && (
        <div className={styles.modalBackdrop} onClick={() => setDeleteConfirmTarget(null)}>
          <div className={styles.modalBox} style={{ maxWidth: 400, padding: 24, textAlign: 'center' }} onClick={(e) => e.stopPropagation()}>
            <h3 style={{ fontSize: 18, marginBottom: 12, color: '#fff' }}>Confirm Delete</h3>
            <p style={{ color: '#94a3b8', fontSize: 13, marginBottom: 20 }}>Are you sure you want to delete this item? This action will remove the record.</p>
            <div style={{ display: 'flex', gap: 12, justifyContent: 'center' }}>
              <button className={styles.btnSecondary} onClick={() => setDeleteConfirmTarget(null)}>Cancel</button>
              <button
                className={styles.btnPrimary}
                style={{ background: '#ef4444' }}
                onClick={() => {
                  const id = deleteConfirmTarget;
                  setDeleteConfirmTarget(null);
                  handleDelete(id);
                }}
              >
                Delete
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
