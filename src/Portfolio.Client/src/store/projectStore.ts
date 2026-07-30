import { create } from 'zustand';
import type { Project } from '../types';
import { PROJECTS } from '../api/staticData';

interface ProjectStore {
  projects: Project[];
  addProject: (p: Omit<Project, 'id' | 'slug' | 'displayOrder'>) => void;
  updateProject: (id: number, p: Partial<Project>) => void;
  deleteProject: (id: number) => void;
}

function toSlug(title: string) {
  return title
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-|-$/g, '');
}

export const useProjectStore = create<ProjectStore>((set) => ({
  projects: PROJECTS,

  addProject: (p) =>
    set((state) => {
      const maxId = Math.max(0, ...state.projects.map((x) => x.id));
      const newProject: Project = {
        ...p,
        id: maxId + 1,
        slug: toSlug(p.title),
        displayOrder: state.projects.length + 1,
      };
      return { projects: [...state.projects, newProject] };
    }),

  updateProject: (id, p) =>
    set((state) => ({
      projects: state.projects.map((x) =>
        x.id === id ? { ...x, ...p, slug: p.title ? toSlug(p.title) : x.slug } : x
      ),
    })),

  deleteProject: (id) =>
    set((state) => ({
      projects: state.projects.filter((x) => x.id !== id),
    })),
}));
