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
      const res = await apiClient.get('/api/profile');
      return res.data;
    },
    staleTime: 5 * 60 * 1000,
  });
}

// ---------- Social Links ----------
export function useSocialLinks() {
  return useQuery({
    queryKey: ['social-links'],
    queryFn: async () => {
      if (!USE_API) return SOCIAL_LINKS;
      const res = await apiClient.get('/api/social-links');
      return res.data;
    },
    staleTime: 5 * 60 * 1000,
  });
}

// ---------- Skills ----------
export function useSkills() {
  return useQuery({
    queryKey: ['skills'],
    queryFn: async () => {
      if (!USE_API) return SKILLS;
      const res = await apiClient.get('/api/skills');
      return res.data;
    },
    staleTime: 5 * 60 * 1000,
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
      const res = await apiClient.get('/api/services');
      return res.data;
    },
    staleTime: 5 * 60 * 1000,
  });
}

// ---------- Education ----------
export function useEducation() {
  return useQuery({
    queryKey: ['education'],
    queryFn: async () => {
      if (!USE_API) return EDUCATION;
      const res = await apiClient.get('/api/education');
      return res.data;
    },
    staleTime: 5 * 60 * 1000,
  });
}

// ---------- Soft Skills / Experiences ----------
export function useSoftSkills() {
  return useQuery({
    queryKey: ['soft-skills'],
    queryFn: async () => {
      if (!USE_API) return SOFT_SKILLS;
      const res = await apiClient.get('/api/experiences');
      return res.data;
    },
    staleTime: 5 * 60 * 1000,
  });
}

// ---------- Projects ----------
export function useProjects(category?: string) {
  return useQuery({
    queryKey: ['projects', category],
    queryFn: async () => {
      if (!USE_API) {
        if (!category || category === '*') return PROJECTS;
        return PROJECTS.filter((p: Project) => p.categories.includes(category as Project['categories'][number]));
      }
      const params = category && category !== '*' ? { category } : {};
      const res = await apiClient.get('/api/projects', { params });
      return res.data;
    },
    staleTime: 5 * 60 * 1000,
  });
}

export function useProject(slug: string) {
  return useQuery({
    queryKey: ['project', slug],
    queryFn: async () => {
      if (!USE_API) return PROJECTS.find((p: Project) => p.slug === slug) ?? null;
      const res = await apiClient.get(`/api/projects/${slug}`);
      return res.data;
    },
    enabled: Boolean(slug),
    staleTime: 5 * 60 * 1000,
  });
}

// ---------- Blog ----------
export function useBlogPosts() {
  return useQuery({
    queryKey: ['blog'],
    queryFn: async () => {
      if (!USE_API) return BLOG_POSTS;
      const res = await apiClient.get('/api/blog');
      return res.data;
    },
    staleTime: 5 * 60 * 1000,
  });
}

export function useBlogPost(slug: string) {
  return useQuery({
    queryKey: ['blog', slug],
    queryFn: async () => {
      if (!USE_API) return BLOG_POSTS.find((p: BlogPost) => p.slug === slug) ?? null;
      const res = await apiClient.get(`/api/blog/${slug}`);
      return res.data;
    },
    enabled: Boolean(slug),
    staleTime: 5 * 60 * 1000,
  });
}

// ---------- Contact ----------
export function useContactMutation() {
  return useMutation<ContactResponse, Error, ContactMessage>({
    mutationFn: async (data) => {
      const res = await apiClient.post('/api/contact', data);
      return res.data;
    },
  });
}
