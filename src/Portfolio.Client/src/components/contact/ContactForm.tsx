import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useContactMutation } from '../../api/queries';
import styles from './ContactForm.module.css';

const schema = z.object({
  name:    z.string().min(2, 'Name must be at least 2 characters'),
  email:   z.string().email('Enter a valid email address'),
  subject: z.string().min(3, 'Subject must be at least 3 characters'),
  message: z.string().min(10, 'Message must be at least 10 characters'),
});
type FormValues = z.infer<typeof schema>;

export function ContactForm() {
  const { mutate, isPending, isSuccess, reset: resetMut } = useContactMutation();
  const { register, handleSubmit, reset: resetForm, formState: { errors } } = useForm<FormValues>({ resolver: zodResolver(schema) });

  const onSubmit = (data: FormValues) => mutate(data, { onSuccess: () => resetForm() });

  if (isSuccess) return (
    <div className={styles.success} role="alert">
      <span className={styles.successIcon}>✓</span>
      <h3>Message sent!</h3>
      <p>I'll get back to you soon.</p>
      <button className="btn-outline" onClick={() => resetMut()} style={{marginTop: 12}}>Send another</button>
    </div>
  );

  return (
    <form className={styles.form} onSubmit={handleSubmit(onSubmit)} noValidate aria-label="Contact form">
      <div className={styles.row}>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="cn">Your name</label>
          <input id="cn" className={`${styles.input} ${errors.name ? styles.err : ''}`} placeholder="Alice Smith" autoComplete="name" {...register('name')} />
          {errors.name && <span className={styles.errMsg} role="alert">{errors.name.message}</span>}
        </div>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="ce">Email address</label>
          <input id="ce" type="email" className={`${styles.input} ${errors.email ? styles.err : ''}`} placeholder="alice@company.com" autoComplete="email" {...register('email')} />
          {errors.email && <span className={styles.errMsg} role="alert">{errors.email.message}</span>}
        </div>
      </div>
      <div className={styles.field}>
        <label className={styles.label} htmlFor="cs">Subject</label>
        <input id="cs" className={`${styles.input} ${errors.subject ? styles.err : ''}`} placeholder="Let's build something together" {...register('subject')} />
        {errors.subject && <span className={styles.errMsg} role="alert">{errors.subject.message}</span>}
      </div>
      <div className={styles.field}>
        <label className={styles.label} htmlFor="cm">Message</label>
        <textarea id="cm" rows={5} className={`${styles.input} ${styles.textarea} ${errors.message ? styles.err : ''}`} placeholder="Tell me about your project…" {...register('message')} />
        {errors.message && <span className={styles.errMsg} role="alert">{errors.message.message}</span>}
      </div>
      <button type="submit" className={`btn-primary ${styles.submitBtn}`} disabled={isPending} aria-busy={isPending}>
        {isPending ? <><span className={styles.spinner} />Sending…</> : 'Send message →'}
      </button>
    </form>
  );
}
