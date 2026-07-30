# API Design

**Base URL:** `https://api.codersatyam.com` (production) / `http://localhost:5000` (dev)  
**Version:** v1  
**Format:** JSON (camelCase)  
**Auth:** None (public read-only API). Write endpoints (contact) use rate limiting.

---

## Endpoints

### Profile

```http
GET /api/profile
```
Returns the portfolio owner's profile.

**Response 200:**
```json
{
  "id": 1,
  "fullName": "Satyam Kumar",
  "title": "Web Developer",
  "subtitle": "App Developer",
  "aboutText": "Spirited software engineer...",
  "phone": "+91 9113394936",
  "email": "sirsatyamchaudhary@gmail.com",
  "website": "www.codersatyam.com",
  "city": "Chennai",
  "country": "INDIA",
  "age": 20,
  "degree": "Bachelor of Engineering",
  "freelanceStatus": "Available",
  "profileImageUrl": "/assets/images/profile.jpg",
  "cvUrl": "https://drive.google.com/...",
  "mapLat": 43.053454,
  "mapLng": -76.144508
}
```

---

### Skills

```http
GET /api/skills
```
Returns technical and language skills ordered by category and display order.

**Response 200:**
```json
[
  { "id": 1, "name": "Web Design", "percentage": 75, "category": "technical", "displayOrder": 1 },
  { "id": 4, "name": "Hindi", "percentage": 95, "category": "language", "languageLevel": "Expert", "filledDots": 9, "totalDots": 10, "displayOrder": 1 }
]
```

---

### Projects

```http
GET /api/projects
GET /api/projects?category=webdesign
GET /api/projects/{slug}
```

**Query parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `category` | string | Filter by category: `webdesign`, `mobiledesign`, `webapp`, `gamedesign` |

**Response 200 (list):**
```json
[
  {
    "id": 1,
    "slug": "tutor-finder",
    "title": "Tutor Finder",
    "description": "A platform connecting students with tutors...",
    "imageUrl": "/assets/images/project-tutor-finder.png",
    "categories": ["webdesign", "webapp"],
    "liveUrl": null,
    "technologies": ["React", "Node.js", "MongoDB"],
    "displayOrder": 1
  }
]
```

**Response 404 (slug not found):**
```json
{ "detail": "Project 'nonexistent' not found." }
```

---

### Blog

```http
GET /api/blog
GET /api/blog?page=1&pageSize=10
GET /api/blog/{slug}
```

Pagination info is returned in the `X-Pagination` response header:
```json
{ "totalCount": 4, "page": 1, "pageSize": 10, "totalPages": 1 }
```

---

### Social Links

```http
GET /api/social-links
```

---

### Site Settings

```http
GET /api/site-settings
```
Returns a flat `{ key: value }` dictionary.

---

### Contact

```http
POST /api/contact
Content-Type: application/json
```

**Request body:**
```json
{
  "name": "Alice Smith",
  "email": "alice@example.com",
  "subject": "Let's work together",
  "message": "Hi Satyam, I'd love to discuss a project..."
}
```

**Validation rules:**
| Field | Rules |
|-------|-------|
| `name` | Required, 2–100 chars |
| `email` | Required, valid email, max 254 chars |
| `subject` | Required, 3–200 chars |
| `message` | Required, 10–2000 chars |

**Response 200:**
```json
{ "success": true, "message": "Message sent successfully." }
```

**Response 400 (validation failure):**
Standard RFC 7807 Problem Details with `errors` map.

**Response 429 (rate limit):**
Returned when more than 5 contact submissions are made from the same IP within 10 minutes.

---

## Rate Limits

| Endpoint | Limit |
|----------|-------|
| All GET endpoints | 100 req/min per IP |
| `POST /api/contact` | 5 req per 10 min per IP |

---

## Error Format

All errors follow RFC 7807 Problem Details:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "errors": {
    "email": ["Please provide a valid email address."]
  }
}
```

---

## Health Check

```http
GET /health
```
```json
{
  "status": "Healthy",
  "checks": [{ "name": "database", "status": "Healthy" }],
  "totalDuration": "00:00:00.0043210"
}
```

---

## Caching Strategy

| Endpoint | Cache-Control | Duration |
|----------|--------------|----------|
| `/api/profile` | `public, max-age=300` | 5 min |
| `/api/skills` | `public, max-age=300` | 5 min |
| `/api/projects` | `public, max-age=300` | 5 min |
| `/api/blog` | `public, max-age=300` | 5 min |
| `/api/social-links` | `public, max-age=600` | 10 min |
| `/api/site-settings` | `public, max-age=600` | 10 min |
| `POST /api/contact` | No cache | — |
