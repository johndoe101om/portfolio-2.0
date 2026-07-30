import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ContactForm } from '../ContactForm';

vi.mock('../../../api/queries', () => ({
  useContactMutation: () => ({
    mutate: vi.fn(),
    isPending: false,
    isSuccess: false,
    isError: false,
    error: null,
    reset: vi.fn(),
  }),
}));

function wrap(ui: React.ReactElement) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(<QueryClientProvider client={qc}>{ui}</QueryClientProvider>);
}

describe('ContactForm', () => {
  it('renders all form fields', () => {
    wrap(<ContactForm />);
    expect(screen.getByPlaceholderText(/Alice Smith/i)).toBeInTheDocument();
    expect(screen.getByPlaceholderText(/alice@company/i)).toBeInTheDocument();
    expect(screen.getByPlaceholderText(/build something/i)).toBeInTheDocument();
    expect(screen.getByPlaceholderText(/Tell me about/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /Send message/i })).toBeInTheDocument();
  });

  it('shows validation errors when submitting empty form', async () => {
    const user = userEvent.setup();
    wrap(<ContactForm />);
    await user.click(screen.getByRole('button', { name: /Send message/i }));
    await waitFor(() => {
      expect(screen.getByText(/Name must be at least/i)).toBeInTheDocument();
    });
  });

  it('shows error for invalid email', async () => {
    const user = userEvent.setup();
    wrap(<ContactForm />);
    await user.type(screen.getByPlaceholderText(/alice@company/i), 'invalid-email');
    await user.click(screen.getByRole('button', { name: /Send message/i }));
    await waitFor(() => {
      expect(screen.getByText(/valid email/i)).toBeInTheDocument();
    });
  });

  it('does not submit with a message that is too short', async () => {
    const user = userEvent.setup();
    wrap(<ContactForm />);
    await user.type(screen.getByPlaceholderText(/Alice Smith/i), 'Alice');
    await user.type(screen.getByPlaceholderText(/alice@company/i), 'alice@example.com');
    await user.type(screen.getByPlaceholderText(/build something/i), 'Hello');
    await user.type(screen.getByPlaceholderText(/Tell me about/i), 'Hi');
    await user.click(screen.getByRole('button', { name: /Send message/i }));
    await waitFor(() => {
      expect(screen.getByText(/at least 10 characters/i)).toBeInTheDocument();
    });
  });

  it('submit button is accessible', () => {
    wrap(<ContactForm />);
    const btn = screen.getByRole('button', { name: /Send message/i });
    expect(btn).toBeInTheDocument();
    expect(btn).not.toBeDisabled();
  });
});
