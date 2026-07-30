# Security Policy

## Supported Versions

| Version | Supported |
|---------|-----------|
| Latest on `main` | ✅ |

## Reporting a Vulnerability

Please **do not** open a public GitHub issue for security vulnerabilities.

Instead, email: **sirsatyamchaudhary@gmail.com**

Include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (optional)

You will receive a response within 72 hours. Once verified, we aim to release a fix within 7 days for critical issues.

## Security Measures in This Project

- EmailJS credentials removed from client-side code (remediated from original)
- SMTP credentials stored in environment variables only
- Rate limiting on contact form (5 submissions per 10 minutes per IP)
- Input validation on both client and server
- Security HTTP headers (CSP, HSTS, X-Frame-Options, etc.)
- PostgreSQL credentials never hardcoded
- `.env` files excluded via `.gitignore`
