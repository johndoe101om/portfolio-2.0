import { useState } from 'react';
import { ExternalLink, X, Calendar, Clock, Award, Star, BookOpen, Layers, CheckCircle } from 'lucide-react';
import styles from './PortfolioSection.module.css';
import { useProjects } from '../../api/queries';
import type { Project } from '../../types';

const FILTERS: { value: string; label: string }[] = [
  { value: '*',            label: 'All' },
  { value: 'webdesign',    label: 'Web Design' },
  { value: 'webapp',       label: 'Web Apps' },
  { value: 'mobiledesign', label: 'Mobile' },
  { value: 'gamedesign',   label: 'Game' },
];

export function PortfolioSection() {
  const [activeFilter, setActiveFilter] = useState('*');
  const [selectedProject, setSelectedProject] = useState<Project | null>(null);

  const { data, isLoading } = useProjects(activeFilter);
  const projects = (data ?? []) as Project[];

  return (
    <section aria-labelledby="portfolio-heading" className={styles.wrap}>
      <div className={styles.header}>
        <div>
          <div className="section-eyebrow">My Work & Portfolio</div>
          <h2 id="portfolio-heading" className="section-title">Featured Projects</h2>
        </div>
        <p className={styles.count}>{projects.length} project{projects.length !== 1 ? 's' : ''}</p>
      </div>

      <div role="tablist" aria-label="Filter by category" className={styles.filterBar}>
        {FILTERS.map((f) => (
          <button
            key={f.value}
            role="tab"
            aria-selected={activeFilter === f.value}
            className={`${styles.filterBtn} ${activeFilter === f.value ? styles.filterActive : ''}`}
            onClick={() => setActiveFilter(f.value)}
          >
            {f.label}
          </button>
        ))}
      </div>

      <div
        className={styles.grid}
        role="tabpanel"
        aria-label={`${FILTERS.find((f) => f.value === activeFilter)?.label ?? 'All'} projects`}
      >
        {isLoading && (
          <p className={styles.empty}>Loading projects from API...</p>
        )}

        {!isLoading && projects.map((p) => (
          <article
            key={p.id}
            className={`${styles.card} glass-card`}
            onClick={() => setSelectedProject(p)}
          >
            <div className={styles.thumb}>
              <img
                src={p.thumbnailUrl || p.imageUrl || '/assets/images/placeholder.png'}
                alt={p.title}
                className={styles.thumbImg}
              />
              <div className={styles.badgeRow}>
                {p.status && <span className={styles.statusBadge}>{p.status}</span>}
                {p.isFeatured && <span className={styles.featuredBadge}><Star size={10} style={{ display: 'inline' }} /> Featured</span>}
              </div>
            </div>

            <div className={styles.body}>
              <h3 className={styles.cardTitle}>{p.title}</h3>
              <p className={styles.cardDesc}>{p.shortDescription || p.description}</p>
              
              <div className={styles.techs}>
                {p.technologies?.slice(0, 4).map((t) => (
                  <span key={t} className={styles.tech}>{t}</span>
                ))}
                {p.technologies?.length > 4 && (
                  <span className={styles.tech}>+{p.technologies.length - 4}</span>
                )}
              </div>

              <div className={styles.cardFooter}>
                <span>{p.durationText || 'Completed'}</span>
                <span className={styles.liveLink}>
                  Details <ExternalLink size={12} />
                </span>
              </div>
            </div>
          </article>
        ))}

        {!isLoading && projects.length === 0 && (
          <p className={styles.empty}>No projects found in this category.</p>
        )}
      </div>

      {/* PUBLIC PROJECT DETAIL MODAL */}
      {selectedProject && (
        <div className={styles.modalBackdrop} onClick={() => setSelectedProject(null)}>
          <div className={styles.modalContent} onClick={(e) => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <img
                src={selectedProject.thumbnailUrl || selectedProject.imageUrl || '/assets/images/placeholder.png'}
                alt={selectedProject.title}
                className={styles.modalHeroImg}
              />
              <div className={styles.modalHeroOverlay} />
              <button
                className={styles.modalCloseBtn}
                onClick={() => setSelectedProject(null)}
                aria-label="Close"
              >
                <X size={20} />
              </button>

              <div className={styles.modalHeaderTitle}>
                <h2>{selectedProject.title}</h2>
                <div className={styles.modalHeaderMeta}>
                  <span><Layers size={13} style={{ display: 'inline' }} /> {selectedProject.resumeCategory || 'Web'}</span>
                  <span>•</span>
                  <span><Clock size={13} style={{ display: 'inline' }} /> {selectedProject.durationText}</span>
                  {selectedProject.status && (
                    <>
                      <span>•</span>
                      <span className={styles.statusBadge}>{selectedProject.status}</span>
                    </>
                  )}
                </div>
              </div>
            </div>

            <div className={styles.modalBody}>
              {/* Overview */}
              <div className={styles.modalSection}>
                <h4 className={styles.modalSectionTitle}><BookOpen size={16} /> Overview</h4>
                <p style={{ color: '#cbd5e1', lineHeight: '1.6' }}>
                  {selectedProject.fullDescription || selectedProject.shortDescription || selectedProject.description}
                </p>
              </div>

              {/* Technologies */}
              {selectedProject.technologies && selectedProject.technologies.length > 0 && (
                <div className={styles.modalSection}>
                  <h4 className={styles.modalSectionTitle}><Layers size={16} /> Technologies & Stack</h4>
                  <div className={styles.techs}>
                    {selectedProject.technologies.map((t) => (
                      <span key={t} className={styles.tech} style={{ fontSize: '13px', padding: '5px 12px' }}>{t}</span>
                    ))}
                  </div>
                </div>
              )}

              {/* Gallery Images */}
              {selectedProject.images && selectedProject.images.length > 0 && (
                <div className={styles.modalSection}>
                  <h4 className={styles.modalSectionTitle}>Project Gallery</h4>
                  <div className={styles.galleryGrid}>
                    {selectedProject.images.map((img, idx) => (
                      <img
                        key={img.id || idx}
                        src={img.publicUrl}
                        alt={img.altText || `Gallery image ${idx + 1}`}
                        className={styles.galleryImg}
                        onClick={() => window.open(img.publicUrl, '_blank')}
                      />
                    ))}
                  </div>
                </div>
              )}

              {/* Key Features */}
              {selectedProject.features && selectedProject.features.length > 0 && (
                <div className={styles.modalSection}>
                  <h4 className={styles.modalSectionTitle}><CheckCircle size={16} /> Key Features</h4>
                  <div className={styles.featureList}>
                    {selectedProject.features.map((f, i) => (
                      <div key={f.id || i} className={styles.featureItem}>
                        <div className={styles.featureTitle}>{f.title}</div>
                        {f.description && <div className={styles.featureDesc}>{f.description}</div>}
                      </div>
                    ))}
                  </div>
                </div>
              )}

              {/* Achievements */}
              {selectedProject.achievements && selectedProject.achievements.length > 0 && (
                <div className={styles.modalSection}>
                  <h4 className={styles.modalSectionTitle}><Award size={16} /> Key Milestones</h4>
                  <div className={styles.achievementList}>
                    {selectedProject.achievements.map((a, i) => (
                      <div key={a.id || i} className={styles.achievementItem}>
                        <div className={styles.achievementDot} />
                        <div style={{ fontWeight: 600, color: '#f1f5f9' }}>{a.title}</div>
                        {a.description && <div style={{ fontSize: '12px', color: '#94a3b8' }}>{a.description}</div>}
                      </div>
                    ))}
                  </div>
                </div>
              )}

              {/* README */}
              {selectedProject.readmeMarkdown && (
                <div className={styles.modalSection}>
                  <h4 className={styles.modalSectionTitle}><BookOpen size={16} /> README Documentation</h4>
                  <div className={styles.readmeBox}>
                    <pre style={{ fontFamily: 'monospace', whiteSpace: 'pre-wrap' }}>{selectedProject.readmeMarkdown}</pre>
                  </div>
                </div>
              )}

              {/* External Links */}
              {((selectedProject.links && selectedProject.links.length > 0) || selectedProject.liveUrl) && (
                <div className={styles.modalSection}>
                  <h4 className={styles.modalSectionTitle}>Project Links</h4>
                  <div className={styles.linksRow}>
                    {selectedProject.liveUrl && (
                      <a href={selectedProject.liveUrl} target="_blank" rel="noopener noreferrer" className={styles.linkBtn}>
                        <ExternalLink size={14} /> Live Demo
                      </a>
                    )}
                    {selectedProject.links?.map((link, i) => (
                      <a key={link.id || i} href={link.url} target="_blank" rel="noopener noreferrer" className={styles.linkBtn}>
                        <ExternalLink size={14} /> {link.label || link.linkType || 'Link'}
                      </a>
                    ))}
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </section>
  );
}
