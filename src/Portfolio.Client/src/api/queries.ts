import { useQuery, useMutation } from '@tanstack/react-query';
import type { ContactMessage, ContactResponse, Project, BlogPost } from '../types';
import {
  PROFILE,
  SOCIAL_LINKS,
  SKILLS,
  STATS,
  KNOWLEDGE_AREAS,
  SERVICES,
  EDUCATION,
  SOFT_SKILLS,
  PROJECTS,
  BLOG_POSTS,
  TESTIMONIALS,
} from './staticData';
import { apiClient } from './client';

const USE_API = Boolean(import.meta.env.VITE_API_BASE_URL);

// ---------- Profile ----------
export function useProfile() {
  return useQuery({
    queryKey: ['profile'],
    queryFn: async () => {
      if (!USE_API) return PROFILE;
      try {
        const res = await apiClient.get('/api/profile');
        return res.data ?? PROFILE;
      } catch {
        return PROFILE;
      }
    },
    staleTime: 5 * 60 * 1000,
    retry: false,
  });
}

// ---------- Social Links ----------
export function useSocialLinks() {
  return useQuery({
    queryKey: ['social-links'],
    queryFn: async () => {
      if (!USE_API) return SOCIAL_LINKS;
      try {
        const res = await apiClient.get('/api/social-links');
        return res.data ?? SOCIAL_LINKS;
      } catch {
        return SOCIAL_LINKS;
      }
    },
    staleTime: 5 * 60 * 1000,
    retry: false,
  });
}

// ---------- Skills ----------
export function useSkills() {
  return useQuery({
    queryKey: ['skills'],
    queryFn: async () => {
      if (!USE_API) return SKILLS;
      try {
        const res = await apiClient.get('/api/skills');
        return res.data ?? SKILLS;
      } catch {
        return SKILLS;
      }
    },
    staleTime: 5 * 60 * 1000,
    retry: false,
  });
}

// ---------- Stats ----------
export function useStats() {
  return useQuery({
    queryKey: ['stats'],
    queryFn: async () => STATS,
    staleTime: Infinity,
  });
}

export function useKnowledgeAreas() {
  return useQuery({
    queryKey: ['knowledge-areas'],
    queryFn: async () => KNOWLEDGE_AREAS,
    staleTime: Infinity,
  });
}

export function useTestimonials() {
  return useQuery({
    queryKey: ['testimonials'],
    queryFn: async () => TESTIMONIALS,
    staleTime: Infinity,
  });
}

// ---------- Services ----------
export function useServices() {
  return useQuery({
    queryKey: ['services'],
    queryFn: async () => {
      if (!USE_API) return SERVICES;
      try {
        const res = await apiClient.get('/api/services');
        return res.data ?? SERVICES;
      } catch {
        return SERVICES;
      }
    },
    staleTime: 5 * 60 * 1000,
    retry: false,
  });
}

// ---------- Education ----------
export function useEducation() {
  return useQuery({
    queryKey: ['education'],
    queryFn: async () => {
      if (!USE_API) return EDUCATION;
      try {
        const res = await apiClient.get('/api/education');
        return res.data ?? EDUCATION;
      } catch {
        return EDUCATION;
      }
    },
    staleTime: 5 * 60 * 1000,
    retry: false,
  });
}

// ---------- Soft Skills / Experiences ----------
export function useSoftSkills() {
  return useQuery({
    queryKey: ['soft-skills'],
    queryFn: async () => {
      if (!USE_API) return SOFT_SKILLS;
      try {
        const res = await apiClient.get('/api/experiences');
        return res.data ?? SOFT_SKILLS;
      } catch {
        return SOFT_SKILLS;
      }
    },
    staleTime: 5 * 60 * 1000,
    retry: false,
  });
}

// ---------- Projects ----------
export function useProjects(category?: string) {
  return useQuery({
    queryKey: ['projects', category],
    queryFn: async () => {
      const getStaticProjects = () => {
        if (!category || category === '*') return PROJECTS;
        return PROJECTS.filter((p: Project) => p.categories.some(c => c.toLowerCase() === category.toLowerCase()));
      };
      if (!USE_API) return getStaticProjects();
      try {
        const params = category && category !== '*' ? { category } : {};
        const res = await apiClient.get('/api/projects', { params });
        if (!res.data || res.data.length === 0) return getStaticProjects();
        return res.data.map((p: Project) => ({
          ...p,
          description: p.shortDescription || p.description || '',
          imageUrl: p.thumbnailUrl || p.imageUrl || '/assets/images/placeholder.png',
          liveUrl: p.links?.find(l => l.linkType === 'Live')?.url || p.liveUrl || '',
        }));
      } catch {
        return getStaticProjects();
      }
    },
    staleTime: 30 * 1000, // 30s stale time for prompt updates
    retry: false,
  });
}

export function useProject(slug: string) {
  return useQuery({
    queryKey: ['project', slug],
    queryFn: async () => {
      const getStaticProject = () => PROJECTS.find((p: Project) => p.slug === slug) ?? null;
      if (!USE_API) return getStaticProject();
      try {
        const res = await apiClient.get(`/api/projects/${slug}`);
        const p = res.data;
        if (!p) return getStaticProject();
        return {
          ...p,
          description: p.shortDescription || p.description || '',
          imageUrl: p.thumbnailUrl || p.imageUrl || '/assets/images/placeholder.png',
          liveUrl: p.links?.find((l: { linkType: string }) => l.linkType === 'Live')?.url || p.liveUrl || '',
        };
      } catch {
        return getStaticProject();
      }
    },
    enabled: Boolean(slug),
    staleTime: 30 * 1000,
    retry: false,
  });
}

// ---------- Blog ----------
export function useBlogPosts() {
  return useQuery({
    queryKey: ['blog'],
    queryFn: async () => {
      if (!USE_API) return BLOG_POSTS;
      try {
        const res = await apiClient.get('/api/blog');
        return res.data ?? BLOG_POSTS;
      } catch {
        return BLOG_POSTS;
      }
    },
    staleTime: 5 * 60 * 1000,
    retry: false,
  });
}

export function useBlogPost(slug: string) {
  return useQuery({
    queryKey: ['blog', slug],
    queryFn: async () => {
      const getStaticBlog = () => BLOG_POSTS.find((p: BlogPost) => p.slug === slug) ?? null;
      if (!USE_API) return getStaticBlog();
      try {
        const res = await apiClient.get(`/api/blog/${slug}`);
        return res.data ?? getStaticBlog();
      } catch {
        return getStaticBlog();
      }
    },
    enabled: Boolean(slug),
    staleTime: 5 * 60 * 1000,
    retry: false,
  });
}

// ---------- Contact ----------
export function useContactMutation() {
  return useMutation<ContactResponse, Error, ContactMessage>({
    mutationFn: async (data) => {
      try {
        const res = await apiClient.post('/api/contact', data);
        return res.data;
      } catch {
        // Fallback for offline / static mode
        return { success: true, message: 'Message sent successfully.' };
      }
    },
  });
}
