# GothamPost Blog API 🦇📰

Welcome to **GothamPost Blog API**, a .NET-based blogging platform with **user authentication**, **blog posts**, **comments**, and **likes**. This API allows users to create and interact with blog posts in a Dockerized environment.

## Features
- **User authentication** (Register/Login)
- **Blog post creation and retrieval**
- **Commenting on posts**
- **Liking posts**
- **SQLite database with automatic migrations**
- **Docker support for easy deployment**

---

## Prerequisites
Before running the project, ensure you have:
- **Docker** installed ([Download here](https://www.docker.com/))
- **Git** installed ([Download here](https://git-scm.com/))
---

## Setup & Run the Project
### ** 1.Clone the Repository**
```bash
git clone https://github.com/yourusername/WebDev.git
cd WebDev
```

### **2.Build and Run the Docker Container**
```bash
docker build -t bloggingplatformapi .
docker run -d -p 5113:8080 bloggingplatformapi
```

### **3.Verify API is Running**
Open your browser or use `curl`:
```bash
curl -X GET http://localhost:5113/api/blogpost
```

Expected response (sample data):
```json
{
  "$id": "1",
  "$values": [
    {
      "blogPostId": 1,
      "title": "The Batman Strikes Again: Joker's Henchmen Captured!",
      "content": "Late last night, Gotham's vigilante took down an entire gang of Joker’s men...",
      "dateCreated": "2025-02-26T23:23:48.726143",
      "userId": 8,
      "categoryId": 1
    }
  ]
}
```

---

## Database Migrations
### **Automatic Migrations Enabled **
Migrations **run automatically** at startup, so there’s **no need** to manually run `dotnet ef database update`.

If you ever need to reset the database:
```bash
rm bloggingplatform.db
docker stop $(docker ps -q)
docker build -t bloggingplatformapi .
docker run -d -p 5113:8080 bloggingplatformapi
```
This will recreate the database and apply **all existing migrations**.

---

## API Endpoints
### **User Authentication**
| Method | Endpoint | Description |
|--------|-------------|----------------|
| `POST` | `/api/auth/register` | Register a new user |
| `POST` | `/api/auth/login` | Login and receive a JWT token |

### **Blog Posts**
| Method | Endpoint | Description |
|--------|-------------|----------------|
| `GET`  | `/api/blogpost` | Get all blog posts |
| `POST` | `/api/blogpost` | Create a new blog post |

### **Comments**
| Method | Endpoint | Description |
|--------|-------------|----------------|
| `GET`  | `/api/comment` | Get all comments |
| `POST` | `/api/comment` | Add a comment to a post |

### **Likes**
| Method | Endpoint | Description |
|--------|-------------|----------------|
| `POST` | `/api/like` | Like a post |

---

## Stopping and Restarting the API
To stop the running container:
```bash
docker stop $(docker ps -q)
```
To restart:
```bash
docker run -d -p 5113:8080 bloggingplatformapi
```

---

## Notes
- The **database file (`bloggingplatform.db`) is included**, so existing data (users, posts, comments, likes) is **already available** when running the project.
- Ensure port **5113** is available before running the container.
- If the database is not found, **automatic migrations** will recreate the tables.

---

**Enjoy Gotham Post!** 🦇
