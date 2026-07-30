import styles from './BlogSection.module.css';
import { useBlogPosts } from '../../api/queries';
import type { BlogPost } from '../../types';

const EMOJIS: Record<string, string> = {
  'best-way-to-become-good-web-designer': '🎨',
  'enhancing-coding-logic': '💡',
  'practices-for-personal-and-professional-growth': '🌱',
  'how-to-crack-any-technical-interview': '🎯',
};

function fmt(d: string) {
  return new Date(d).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' });
}

export function BlogSection() {
  const { data: posts = [], isLoading } = useBlogPosts();
  return (
    <section aria-labelledby="blog-heading" className={styles.wrap}>
      <div className={styles.header}>
        <div className="section-eyebrow">Thoughts & articles</div>
        <h2 id="blog-heading" className="section-title">Latest posts</h2>
      </div>
      {isLoading ? (
        <div className={styles.loading} aria-live="polite">Loading…</div>
      ) : (
        <div className={styles.grid}>
          {(posts as BlogPost[]).map((p) => (
            <article key={p.id} className={`${styles.card} glass-card`}>
              <div className={styles.thumb} style={{background:`linear-gradient(135deg,rgba(139,92,246,.1),rgba(0,212,255,.06))`}}>
                <span className={styles.thumbEmoji}>{EMOJIS[p.slug] ?? '📝'}</span>
                <time dateTime={p.publishedAt} className={styles.date}>{fmt(p.publishedAt)}</time>
              </div>
              <div className={styles.body}>
                <div className={styles.tags}>
                  {p.tags.map((t) => <span key={t} className={styles.tag}>{t}</span>)}
                </div>
                <h3 className={styles.title}>{p.title}</h3>
                <p className={styles.excerpt}>{p.excerpt}</p>
                <span className={styles.readMore}>Read more →</span>
              </div>
            </article>
          ))}
        </div>
      )}
    </section>
  );
}
