# GothamPost Blog API 🦇📰

## **🚀 A .NET-based blogging platform** with authentication, posts, comments, and likes.
## **❤️ Dockerized** for easy deployment.  
## ✨ Secure with JWT Authentication.  
---
## **Features**  
✅ User Authentication (Register/Login)  
✅ Blog Post Creation & Retrieval  
✅ Commenting & Liking Posts  
✅ SQLite Database with Automatic Migrations  
✅ Docker Support for Easy Deployment  
---
## **Prerequisites**  
Before running the project, ensure you have:  
- **Docker installed** ([Download Here](https://www.docker.com/get-started))  
- **Git installed** ([Download Here](https://git-scm.com/downloads))  
---

## **🚀 Setup & Run the Project**  

### **1.Clone the Repository**  
```sh
git clone https://github.com/yourusername/WebDev.git
cd WebDev
```

### **2.Build and Run the Docker Container**  
```sh
docker build -t bloggingplatformapi .
docker run -d -p 5113:8080 bloggingplatformapi
```

### **3.Verify API is Running**  
Open your browser or use curl:  
```sh
curl -X GET http://localhost:5113/api/blogpost
```

Expected Response (Sample Data):  
```json
{
  "blogPostId": 1,
  "title": "The Batman Strikes Again",
  "content": "Gotham's vigilante took down Joker’s men...",
  "dateCreated": "2025-02-26T23:23:48.726143",
  "userId": 8,
  "categoryId": 1
}
```

---
## **API Documentation (Swagger UI)**  
**Swagger UI:** [http://localhost:5113/swagger](http://localhost:5113/swagger)  
Use Swagger UI to test API endpoints directly in your browser.  
---

## **Database & Migrations**  

**Automatic Migrations Enabled**  
Migrations **run automatically at startup**.  

### **Reset the Database (If Needed)**  
```sh
rm bloggingplatform.db
docker stop $(docker ps -q)
docker build -t bloggingplatformapi .
docker run -d -p 5113:8080 bloggingplatformapi
```
---

## **Environment Variables & Secrets**  

⚠️ **Important:** Set the following **secrets/environment variables** in `appsettings.json` or GitHub Actions.  

```json
{
  "Jwt": {
    "SecretKey": "YOUR_SECRET_KEY",
    "Issuer": "GothamPost",
    "Audience": "GothamUsers"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=bloggingplatform.db"
  }
}
```
---

## **API Endpoints**  

### **🔐 User Authentication**  
| Method | Endpoint | Description |
|--------|---------|------------|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and receive a JWT token |

### **📝 Blog Posts**  
| Method | Endpoint | Description |
|--------|---------|------------|
| GET | `/api/blogpost` | Get all blog posts |
| POST | `/api/blogpost` | Create a new blog post |

### **💬 Comments**  
| Method | Endpoint | Description |
|--------|---------|------------|
| GET | `/api/comment` | Get all comments |
| POST | `/api/comment` | Add a comment to a post |

### **❤️ Likes**  
| Method | Endpoint | Description |
|--------|---------|------------|
| POST | `/api/like` | Like a post |

---
## **Stopping & Restarting the API**  

### **Stop the Running Container**  
```sh
docker stop $(docker ps -q)
```
### **Restart the API**  
```sh
docker run -d -p 5113:8080 bloggingplatformapi
```
---

## **Notes**  
✅ The database file (`bloggingplatform.db`) **persists data** across restarts.  
✅ Ensure **port 5113** is available before running the container.  
✅ If the database is missing, **automatic migrations will recreate it**.  

**Enjoy Gotham Post!** 🦇 
