import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { MobileNavigation } from '../MobileNavigation';
import { NAV_ITEMS } from '../../../api/staticData';

const defaultProps = {
  isOpen: false,
  activeSection: 'hero' as const,
  navItems: NAV_ITEMS,
  profileName: 'Satyam Kumar',
  profileImageUrl: '/img/profile.jpg',
  cvUrl: '/cv.pdf',
  onToggle: vi.fn(),
  onNavigate: vi.fn(),
  onPrev: vi.fn(),
  onNext: vi.fn(),
};

describe('MobileNavigation', () => {
  it('renders hamburger button', () => {
    render(<MobileNavigation {...defaultProps} />);
    expect(screen.getByRole('button', { name: /Open menu/i })).toBeInTheDocument();
  });

  it('hamburger aria-expanded is false when closed', () => {
    render(<MobileNavigation {...defaultProps} isOpen={false} />);
    expect(screen.getByRole('button', { name: /Open menu/i }))
      .toHaveAttribute('aria-expanded', 'false');
  });

  it('hamburger aria-expanded is true when open', () => {
    render(<MobileNavigation {...defaultProps} isOpen={true} />);
    expect(screen.getByRole('button', { name: /Close menu/i }))
      .toHaveAttribute('aria-expanded', 'true');
  });

  it('calls onToggle when hamburger clicked', async () => {
    const onToggle = vi.fn();
    const user = userEvent.setup();
    render(<MobileNavigation {...defaultProps} onToggle={onToggle} />);
    await user.click(screen.getByRole('button', { name: /Open menu/i }));
    expect(onToggle).toHaveBeenCalledOnce();
  });

  it('shows nav items when sidebar is open', () => {
    render(<MobileNavigation {...defaultProps} isOpen={true} />);
    expect(screen.getByText('Home')).toBeInTheDocument();
    expect(screen.getByText('About')).toBeInTheDocument();
    expect(screen.getByText('Portfolio')).toBeInTheDocument();
  });

  it('calls onNavigate and onToggle when nav link clicked', async () => {
    const onNavigate = vi.fn();
    const onToggle = vi.fn();
    const user = userEvent.setup();
    render(<MobileNavigation {...defaultProps} isOpen={true} onNavigate={onNavigate} onToggle={onToggle} />);
    await user.click(screen.getByText('About'));
    expect(onNavigate).toHaveBeenCalledWith('about');
    expect(onToggle).toHaveBeenCalled();
  });

  it('marks active nav item with aria-current=page', () => {
    render(<MobileNavigation {...defaultProps} isOpen={true} activeSection="about" />);
    const aboutLink = screen.getAllByText('About')[0].closest('a');
    expect(aboutLink).toHaveAttribute('aria-current', 'page');
  });

  it('renders prev/next navigation buttons', () => {
    render(<MobileNavigation {...defaultProps} />);
    expect(screen.getByRole('button', { name: /Previous section/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /Next section/i })).toBeInTheDocument();
  });
});
