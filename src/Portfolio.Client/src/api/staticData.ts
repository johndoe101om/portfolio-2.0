/**
 * Static seed data extracted from the original portfolio HTML.
 * This file is used during development when the ASP.NET Core API
 * is not running. Replace API_BASE_URL with the real endpoint
 * and remove this file when the backend is fully deployed.
 */

import type {
  Profile,
  SocialLink,
  Skill,
  StatItem,
  Service,
  Education,
  Experience,
  Project,
  BlogPost,
  TestimonialItem,
  KnowledgeArea,
  NavItem,
} from '../types';

export const PROFILE: Profile = {
  id: 1,
  fullName: 'Satyam Kumar',
  title: 'Web Developer',
  subtitle: 'App Developer',
  aboutText:
    'Spirited software engineer with a love for clean code and problem-solving. Always exploring new technologies and methodologies to enhance development efficiency. Driven by a desire to create robust, scalable, and user-friendly software solutions.',
  phone: '+91 9113394936',
  email: 'sirsatyamchaudhary@gmail.com',
  website: 'www.codersatyam.com',
  city: 'Chennai',
  country: 'INDIA',
  age: 20,
  degree: 'Bachelor of Engineering',
  freelanceStatus: 'Available',
  profileImageUrl: '/assets/images/profile.jpg',
  cvUrl:
    'https://drive.google.com/file/d/1P28ffSgcD7xEWpu02UgWMAV1b3kp_fyJ/view?usp=sharing',
  mapLat: 43.053454,
  mapLng: -76.144508,
};

export const SOCIAL_LINKS: SocialLink[] = [
  { id: 1, platform: 'WhatsApp',  url: 'https://wa.me/qr/TZU5O77ZT4MGN1',                   iconClass: 'bi bi-whatsapp',  displayOrder: 1 },
  { id: 2, platform: 'Instagram', url: 'https://www.instagram.com/be_stranger7964/',         iconClass: 'bi bi-instagram', displayOrder: 2 },
  { id: 3, platform: 'LinkedIn',  url: 'https://www.linkedin.com/in/satyam-webdeveloper/',   iconClass: 'bi bi-linkedin',  displayOrder: 3 },
];

export const ROTATING_ROLES: string[] = [
  'App Developer',
  'Web Developer',
  'DevOps Engineer',
  'Cloud Engineer',
];

export const STATS: StatItem[] = [
  { id: 1, iconClass: 'bi bi-palette',        value: 2,  label: 'DevOps Projects',  displayOrder: 1 },
  { id: 2, iconClass: 'bi bi-laptop',         value: 12, label: 'Web Designs',       displayOrder: 2 },
  { id: 3, iconClass: 'bi bi-award',          value: 26, label: 'Web Development',   displayOrder: 3 },
  { id: 4, iconClass: 'bi bi-journal-check',  value: 40, label: 'Projects Done',     displayOrder: 4 },
];

export const KNOWLEDGE_AREAS: KnowledgeArea[] = [
  { id: 1, label: 'Machine Learning' },
  { id: 2, label: 'Data Science' },
  { id: 3, label: 'Software Development' },
  { id: 4, label: 'Teaching Web Design' },
];

export const SKILLS: Skill[] = [
  { id: 1, name: 'Web Design',     percentage: 75, category: 'technical', displayOrder: 1 },
  { id: 2, name: 'Web Developer',  percentage: 90, category: 'technical', displayOrder: 2 },
  { id: 3, name: 'Cloud',          percentage: 85, category: 'technical', displayOrder: 3 },
  { id: 4, name: 'Hindi',   category: 'language', percentage: 95, languageLevel: 'Expert',       filledDots: 9, totalDots: 10, displayOrder: 1 },
  { id: 5, name: 'English', category: 'language', percentage: 80, languageLevel: 'Intermediate',  filledDots: 8, totalDots: 10, displayOrder: 2 },
];

export const TESTIMONIALS: TestimonialItem[] = [
  {
    id: 1,
    quote: '"Design is not just what it looks like and feels like. Design is how it works."',
    authorName: 'Steve Jobs',
    authorTitle: 'Designer',
    authorImageUrl: '/assets/images/testimonial-steve.png',
  },
  {
    id: 2,
    quote: '"Any fool can write code that a computer can understand. Good programmers write code that humans can understand."',
    authorName: 'Martin Fowler',
    authorTitle: 'Developer',
    authorImageUrl: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTtT1e-oQ6PQHr72kZzulDQlAqp0pxVEqo-sg&s',
  },
  {
    id: 3,
    quote: '"Good design is obvious. Great design is transparent!"',
    authorName: 'Joe Sparano',
    authorTitle: 'Web Designer',
    authorImageUrl: '/assets/images/testimonial-joe.jpg',
  },
];

export const SERVICES: Service[] = [
  {
    id: 1,
    title: 'Web Design',
    iconClass: 'bi bi-laptop',
    description:
      'I specialize in creating modern, visually engaging, and user-centered web designs that enhance usability and provide a seamless experience across all devices. My designs emphasize clean aesthetics, intuitive navigation, and responsive layouts to ensure optimal user satisfaction.',
    displayOrder: 1,
  },
  {
    id: 2,
    title: 'DevOps Engineer',
    iconClass: 'fa-solid fa-infinity',
    description:
      'I focus on streamlining development and operational workflows through effective automation, continuous integration, and deployment practices. My experience includes working with tools such as Docker, Kubernetes, Jenkins, and AWS, enabling scalable infrastructure and reducing deployment times.',
    displayOrder: 2,
  },
  {
    id: 3,
    title: 'Web Development',
    iconClass: 'bi bi-award',
    description:
      'I build robust, responsive, and dynamic web applications using the latest technologies and best practices. With expertise in both frontend and backend development, I create seamless user experiences using React, Node.js, and various databases.',
    displayOrder: 3,
  },
  {
    id: 4,
    title: 'Data Visualization',
    iconClass: 'fa-solid fa-database',
    description:
      'I transform complex data into clear, engaging, and interactive visualizations that tell a story and enable informed decision-making. Using tools like D3.js, Chart.js, and Tableau, I create charts, dashboards, and reports that highlight key insights and trends.',
    displayOrder: 4,
  },
  {
    id: 5,
    title: 'Generative AI',
    iconClass: 'fa-solid fa-wand-magic-sparkles',
    description:
      'I work with Generative AI technologies to create innovative, AI-driven solutions that transform ideas into reality. With experience in models like GPT, GANs, and VAEs, I develop applications for content generation, image synthesis, and more.',
    displayOrder: 5,
  },
  {
    id: 6,
    title: 'Game Development',
    iconClass: 'bi bi-controller',
    description:
      'I create immersive and engaging game experiences by combining technical skills with a passion for storytelling and design. With proficiency in game engines like Unity and Unreal Engine, I develop games with compelling mechanics and smooth performance across platforms.',
    displayOrder: 6,
  },
];

export const EDUCATION: Education[] = [
  {
    id: 1,
    institution: 'Dr Sarvapalli Radhakrishnan Shiksha Samrat, Simrahi',
    period: '2009 – 2017',
    description:
      'My schooling years laid a strong foundation in both academics and personal growth. I was actively involved in various extracurricular activities, from science fairs to sports, which helped me develop teamwork, leadership, and problem-solving skills early on.',
    displayOrder: 1,
  },
  {
    id: 2,
    institution: 'Sanskar Bharti Global School, Phulparas',
    period: '2017 – 2019',
    description:
      'During my 9th and 10th grades, I focused on strengthening my academic skills, particularly in Mathematics and Science. These years were pivotal in shaping my discipline and work ethic, as I balanced studies with extracurricular activities.',
    displayOrder: 2,
  },
  {
    id: 3,
    institution: 'B.S.S College, Supaul',
    period: '2019 – 2021',
    description:
      'In my 11th and 12th grades, I delved deeper into Physics, Chemistry, and Mathematics. These years were instrumental in honing my analytical skills and solidifying my interest in technology and engineering.',
    displayOrder: 3,
  },
  {
    id: 4,
    institution: 'B.E in Computer Science and Engineering – Chennai',
    period: '2021 – 2025',
    description:
      'My college years were a time of immense growth, both academically and personally. I immersed myself in courses that deepened my understanding of software development, data science, and technology. Participating in hackathons, tech clubs, and collaborative projects allowed me to apply my skills to real-world challenges.',
    displayOrder: 4,
  },
];

export const SOFT_SKILLS: Experience[] = [
  {
    id: 1,
    title: 'Team Leader',
    category: 'softskill',
    description:
      'During my college years, I had the privilege of serving as a team leader in various projects and initiatives. Leading diverse teams, I facilitated collaboration, encouraged creative problem-solving, and ensured objectives were met efficiently.',
    displayOrder: 1,
  },
  {
    id: 2,
    title: 'Business Development',
    category: 'softskill',
    description:
      'I am passionate about driving growth and fostering relationships through strategic business development initiatives. With experience in market analysis, lead generation, and partnership management, I focus on identifying opportunities that align with organisational goals.',
    displayOrder: 2,
  },
  {
    id: 3,
    title: 'Adaptability',
    category: 'softskill',
    description:
      'I thrive in dynamic and fast-paced environments, demonstrating a strong ability to adapt to changing circumstances and challenges. My adaptability enables me to quickly assess problems, pivot strategies, and implement solutions effectively.',
    displayOrder: 3,
  },
  {
    id: 4,
    title: 'Work Ethic',
    category: 'softskill',
    description:
      'I believe that strong work ethics are fundamental to achieving success and fostering a positive work environment. I am committed to integrity, accountability, and diligence in all my endeavours.',
    displayOrder: 4,
  },
];

export const PROJECTS: Project[] = [
  {
    id: 1,
    slug: 'tutor-finder',
    emoji: '📚',
    title: 'Tutor Finder',
    description: 'A platform connecting students with tutors based on subject, location, and availability.',
    imageUrl: '/assets/images/project-tutor-finder.png',
    categories: ['webdesign', 'webapp'],
    technologies: ['React', 'Node.js', 'MongoDB'],
    displayOrder: 1,
  },
  {
    id: 2,
    slug: 'college-lake',
    emoji: '🏫',
    title: 'CollegeLake',
    description: 'A mobile-friendly college discovery and comparison application for prospective students.',
    imageUrl: '/assets/images/project-college-lake.png',
    categories: ['mobiledesign', 'webapp'],
    technologies: ['React Native', 'Firebase'],
    displayOrder: 2,
  },
  {
    id: 3,
    slug: 'online-signature',
    emoji: '✍️',
    title: 'Online Signature',
    description: 'A web application allowing users to create, customise, and save digital signatures.',
    imageUrl: '/assets/images/project-online-signature.png',
    categories: ['webdesign', 'webapp'],
    technologies: ['JavaScript', 'Canvas API', 'PHP'],
    displayOrder: 3,
  },
  {
    id: 4,
    slug: 'skill-navigator',
    emoji: '🗺️',
    title: 'Skill Navigator App',
    description: 'An application that helps users assess and plan their technology skill development journey.',
    imageUrl: '/assets/images/project-skill-navigator.png',
    categories: ['webdesign'],
    technologies: ['React', 'TailwindCSS', 'Node.js'],
    displayOrder: 4,
  },
  {
    id: 5,
    slug: 'raja-mantri-chor-sipahi',
    title: 'Raja Mantri Chor Sipahi',
    description: 'A digital version of the classic Indian card game with online multiplayer support.',
    imageUrl: '/assets/images/project-game.png',
    categories: ['gamedesign', 'webapp'],
    technologies: ['Unity', 'C#', 'WebGL'],
    displayOrder: 5,
  },
  {
    id: 6,
    slug: 'detailed-portfolio',
    emoji: '🎨',
    title: 'Detailed Portfolio',
    description: 'A mobile-first personal portfolio with animated transitions and project showcases.',
    imageUrl: '/assets/images/project-portfolio.png',
    categories: ['mobiledesign'],
    technologies: ['React', 'SCSS', 'Framer Motion'],
    displayOrder: 6,
  },
];

export const BLOG_POSTS: BlogPost[] = [
  {
    id: 1,
    slug: 'best-way-to-become-good-web-designer',
    title: 'The best way to become a good web designer',
    excerpt:
      'Web design is not just about making things look pretty. It\'s about creating meaningful and intuitive experiences for users. Keep designing, keep experimenting, and most importantly, enjoy the journey!',
    imageUrl: '/assets/images/blog-web-designer.png',
    publishedAt: '2024-06-20',
    author: 'Satyam Kumar',
    tags: ['Web Design', 'Career'],
  },
  {
    id: 2,
    slug: 'enhancing-coding-logic',
    title: 'Enhancing Coding Logic: Practices to Sharpen Your Skills',
    excerpt:
      'In today\'s technology-driven world, coding has become an essential skill across various fields. However, simply knowing a programming language is not enough.',
    imageUrl: '/assets/images/blog-coding-logic.png',
    publishedAt: '2024-07-18',
    author: 'Satyam Kumar',
    tags: ['Programming', 'Best Practices'],
  },
  {
    id: 3,
    slug: 'practices-for-personal-and-professional-growth',
    title: 'Practices for Personal and Professional Growth',
    excerpt:
      'In today\'s fast-paced and competitive world, technical skills alone aren\'t enough to ensure success. Employers increasingly seek individuals who possess strong soft skills.',
    imageUrl: '/assets/images/blog-growth.png',
    publishedAt: '2024-09-12',
    author: 'Satyam Kumar',
    tags: ['Soft Skills', 'Career Growth'],
  },
  {
    id: 4,
    slug: 'how-to-crack-any-technical-interview',
    title: 'How to Crack Any Technical Interview in the IT Sector',
    excerpt:
      'Cracking a technical interview can be a challenging process, but with proper preparation, a clear strategy, and the right mindset, you can navigate through the toughest interviews successfully.',
    imageUrl: '/assets/images/blog-interview.png',
    publishedAt: '2024-09-28',
    author: 'Satyam Kumar',
    tags: ['Interview', 'Career'],
  },
];

export const NAV_ITEMS: NavItem[] = [
  { id: 'hero',      label: 'Home',      iconClass: 'bi bi-house' },
  { id: 'about',     label: 'About',     iconClass: 'bi bi-person' },
  { id: 'resume',    label: 'Resume',    iconClass: 'bi bi-file-earmark-text' },
  { id: 'portfolio', label: 'Portfolio', iconClass: 'bi bi-briefcase' },
  { id: 'blog',      label: 'Blog',      iconClass: 'bi bi-journal-text' },
  { id: 'contact',   label: 'Contact',   iconClass: 'bi bi-envelope' },
];
