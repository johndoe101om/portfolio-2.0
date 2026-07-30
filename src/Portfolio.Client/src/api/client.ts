import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '';

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
  timeout: 10_000,
});

apiClient.interceptors.response.use(
  (res) => res,
  (err) => {
    // Structured error for consumers
    const message =
      err.response?.data?.detail ??
      err.response?.data?.message ??
      err.message ??
      'An unexpected error occurred';
    return Promise.reject(new Error(message));
  }
);
