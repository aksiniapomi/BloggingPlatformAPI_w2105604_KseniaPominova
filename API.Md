# GothamPost Blog API Documentation

This document provides comprehensive information about all available endpoints in the GothamPost Blog API, including request formats, response examples, and authentication requirements.

## Table of Contents

- [Authentication](#authentication)
- [Health Check](#health-check)
- [Blog Posts](#blog-posts)
- [Categories](#categories)
- [Comments](#comments)
- [Likes](#likes)
- [Users](#users)
- [Role-Based Access Control](#role-based-access-control)

## Authentication

The API uses JWT (JSON Web Token) for authentication. To access protected endpoints, you need to include the token in the Authorization header:

```
Authorization: Bearer <your_token>
```

To obtain a token, use the [Login](#login) endpoint.

## Health Check

### Check API Status

```
GET /api/healthcheck
```

**Description:** Verifies if the API is running.

**Authentication Required:** No

**Response Example:**

```json
{
  "status": "API is running!"
}
```

## Blog Posts

### Get All Blog Posts

```
GET /api/blogpost
```

**Description:** Retrieves all blog posts with associated data.

**Authentication Required:** No

**Response Example:**

```json
[
  {
    "blogPostId": 1,
    "title": "The Batman Strikes Again: Joker's Henchmen Captured!",
    "content": "Late last night, Gotham's vigilante took down an entire gang of Joker's men near the Ace Chemicals factory. Authorities arrived at the scene to find the criminals tied up with a Bat symbol painted on the ground. Commissioner Gordon remains silent on whether Batman was involved.",
    "dateCreated": "2025-02-26T23:23:48.726143",
    "userId": 8,
    "username": "adminuser",
    "categoryId": 1,
    "categoryName": "Wanted",
    "comments": [
      {
        "commentId": 1,
        "content": "Oh Batsy, always ruining my fun! But I must say... impressive work!",
        "dateCreated": "2025-02-26T23:42:45.413496",
        "userId": 9,
        "username": "Joker",
        "blogPostId": 1
      }
    ],
    "likes": [
      {
        "likeId": 1,
        "userId": 8,
        "username": "adminuser",
        "blogPostId": 1
      }
    ]
  }
]
```

### Get Blog Post by ID

```
GET /api/blogpost/{id}
```

**Description:** Retrieves a specific blog post by ID.

**Authentication Required:** No

**Parameters:**

- `id` (path parameter): The ID of the blog post to retrieve

**Response Example:**

```json
{
  "blogPostId": 1,
  "title": "The Batman Strikes Again: Joker's Henchmen Captured!",
  "content": "Late last night, Gotham's vigilante took down an entire gang of Joker's men near the Ace Chemicals factory. Authorities arrived at the scene to find the criminals tied up with a Bat symbol painted on the ground. Commissioner Gordon remains silent on whether Batman was involved.",
  "dateCreated": "2025-02-26T23:23:48.726143",
  "userId": 8,
  "username": "adminuser"
}
```

### Create Blog Post

```
POST /api/blogpost
```

**Description:** Creates a new blog post.

**Authentication Required:** Yes (Admin or RegisteredUser roles only)

**Request Body:**

```json
{
  "title": "New Blog Post Title",
  "content": "This is the content of the new blog post.",
  "categoryId": 1
}
```

**Response Example:**

```json
{
  "blogPostId": 2,
  "title": "New Blog Post Title",
  "content": "This is the content of the new blog post.",
  "dateCreated": "2025-03-02T13:15:22.123456",
  "userId": 8,
  "username": "adminuser",
  "categoryId": 1,
  "categoryName": "Wanted"
}
```

### Update Blog Post

```
PUT /api/blogpost/{id}
```

**Description:** Updates an existing blog post.

**Authentication Required:** Yes (Admin or RegisteredUser roles only, and only the author or an admin can update)

**Parameters:**

- `id` (path parameter): The ID of the blog post to update

**Request Body:**

```json
{
  "title": "Updated Blog Post Title",
  "content": "This is the updated content of the blog post.",
  "categoryId": 2
}
```

**Response:** 204 No Content on success

### Delete Blog Post

```
DELETE /api/blogpost/{id}
```

**Description:** Deletes a blog post.

**Authentication Required:** Yes (Admin role only)

**Parameters:**

- `id` (path parameter): The ID of the blog post to delete

**Response:** 204 No Content on success

## Categories

### Get All Categories

```
GET /api/category
```

**Description:** Retrieves all categories with associated blog posts.

**Authentication Required:** No

**Response Example:**

```json
[
  {
    "categoryId": 1,
    "name": "Wanted",
    "blogPosts": [
      {
        "blogPostId": 1,
        "title": "The Batman Strikes Again: Joker's Henchmen Captured!",
        "content": "Late last night, Gotham's vigilante took down an entire gang of Joker's men near the Ace Chemicals factory...",
        "dateCreated": "2025-02-26T23:23:48.726143",
        "userId": 8,
        "categoryId": 1
      }
    ]
  },
  {
    "categoryId": 2,
    "name": "Breaking News",
    "blogPosts": []
  },
  {
    "categoryId": 3,
    "name": "Heroes & Villains",
    "blogPosts": []
  }
]
```

### Get Category by ID

```
GET /api/category/{id}
```

**Description:** Retrieves a specific category by ID.

**Authentication Required:** No

**Parameters:**

- `id` (path parameter): The ID of the category to retrieve

**Response Example:**

```json
{
  "categoryId": 1,
  "name": "Wanted",
  "blogPosts": [
    {
      "blogPostId": 1,
      "title": "The Batman Strikes Again: Joker's Henchmen Captured!",
      "content": "Late last night, Gotham's vigilante took down an entire gang of Joker's men near the Ace Chemicals factory...",
      "dateCreated": "2025-02-26T23:23:48.726143",
      "userId": 8,
      "categoryId": 1
    }
  ]
}
```

### Create Category

```
POST /api/category
```

**Description:** Creates a new category.

**Authentication Required:** Yes (Admin role only)

**Request Body:**

```json
{
  "name": "New Category"
}
```

**Response Example:**

```json
{
  "categoryId": 4,
  "name": "New Category",
  "blogPosts": []
}
```

### Update Category

```
PUT /api/category/{id}
```

**Description:** Updates an existing category.

**Authentication Required:** Yes (Admin role only)

**Parameters:**

- `id` (path parameter): The ID of the category to update

**Request Body:**

```json
{
  "name": "Updated Category Name"
}
```

**Response:** 204 No Content on success

### Delete Category

```
DELETE /api/category/{id}
```

**Description:** Deletes a category.

**Authentication Required:** Yes (Admin role only)

**Parameters:**

- `id` (path parameter): The ID of the category to delete

**Response:** 204 No Content on success

## Comments

### Get All Comments

```
GET /api/comment
```

**Description:** Retrieves all comments with associated data.

**Authentication Required:** No

**Response Example:**

```json
[
  {
    "commentId": 1,
    "commentContent": "Oh Batsy, always ruining my fun! But I must say... impressive work!",
    "dateCreated": "2025-02-26T23:42:45.413496",
    "userId": 9,
    "user": {
      "userId": 9,
      "username": "Joker",
      "email": "joker@gotham.com",
      "role": 1
    },
    "blogPostId": 1,
    "blogPost": {
      "blogPostId": 1,
      "title": "The Batman Strikes Again: Joker's Henchmen Captured!",
      "content": "Late last night, Gotham's vigilante took down an entire gang of Joker's men...",
      "dateCreated": "2025-02-26T23:23:48.726143",
      "userId": 8,
      "categoryId": 1
    }
  }
]
```

### Get Comment by ID

```
GET /api/comment/{id}
```

**Description:** Retrieves a specific comment by ID.

**Authentication Required:** No

**Parameters:**

- `id` (path parameter): The ID of the comment to retrieve

**Response Example:**

```json
{
  "commentId": 1,
  "commentContent": "Oh Batsy, always ruining my fun! But I must say... impressive work!",
  "dateCreated": "2025-02-26T23:42:45.413496",
  "userId": 9,
  "user": {
    "userId": 9,
    "username": "Joker",
    "email": "joker@gotham.com",
    "role": 1
  },
  "blogPostId": 1,
  "blogPost": {
    "blogPostId": 1,
    "title": "The Batman Strikes Again: Joker's Henchmen Captured!",
    "content": "Late last night, Gotham's vigilante took down an entire gang of Joker's men...",
    "dateCreated": "2025-02-26T23:23:48.726143",
    "userId": 8,
    "categoryId": 1
  }
}
```

### Create Comment

```
POST /api/comment
```

**Description:** Creates a new comment on a blog post.

**Authentication Required:** Yes (Admin, RegisteredUser, or Reader roles)

**Request Body:**

```json
{
  "commentContent": "This is a test comment",
  "blogPostId": 1
}
```

**Response Example:**

```json
{
  "commentId": 2,
  "commentContent": "This is a test comment",
  "dateCreated": "2025-03-02T13:25:45.123456",
  "userId": 8,
  "blogPostId": 1
}
```

### Update Comment

```
PUT /api/comment/{id}
```

**Description:** Updates an existing comment.

**Authentication Required:** Yes (Admin or RegisteredUser roles only, and only the author or an admin can update)

**Parameters:**

- `id` (path parameter): The ID of the comment to update

**Request Body:**

```json
{
  "commentContent": "This is an updated comment",
  "blogPostId": 1
}
```

**Response:** 204 No Content on success

### Delete Comment

```
DELETE /api/comment/{id}
```

**Description:** Deletes a comment.

**Authentication Required:** Yes (Admin role only)

**Parameters:**

- `id` (path parameter): The ID of the comment to delete

**Response:** 204 No Content on success

## Likes

### Get All Likes

```
GET /api/like
```

**Description:** Retrieves all likes with associated data.

**Authentication Required:** No

**Response Example:**

```json
[
  {
    "likeId": 1,
    "userId": 8,
    "user": {
      "userId": 8,
      "username": "adminuser",
      "email": "admin@example.com",
      "role": 0
    },
    "blogPostId": 1,
    "blogPost": {
      "blogPostId": 1,
      "title": "The Batman Strikes Again: Joker's Henchmen Captured!",
      "content": "Late last night, Gotham's vigilante took down an entire gang of Joker's men...",
      "dateCreated": "2025-02-26T23:23:48.726143",
      "userId": 8,
      "categoryId": 1
    }
  }
]
```

### Get Like by ID

```
GET /api/like/{id}
```

**Description:** Retrieves a specific like by ID.

**Authentication Required:** No

**Parameters:**

- `id` (path parameter): The ID of the like to retrieve

**Response Example:**

```json
{
  "likeId": 1,
  "userId": 8,
  "user": {
    "userId": 8,
    "username": "adminuser",
    "email": "admin@example.com",
    "role": 0
  },
  "blogPostId": 1,
  "blogPost": {
    "blogPostId": 1,
    "title": "The Batman Strikes Again: Joker's Henchmen Captured!",
    "content": "Late last night, Gotham's vigilante took down an entire gang of Joker's men...",
    "dateCreated": "2025-02-26T23:23:48.726143",
    "userId": 8,
    "categoryId": 1
  }
}
```

### Create Like

```
POST /api/like
```

**Description:** Adds a like to a blog post.

**Authentication Required:** Yes (Admin, RegisteredUser, or Reader roles)

**Request Body:**

```json
{
  "blogPostId": 1
}
```

**Response Example:**

```json
{
  "likeId": 3,
  "userId": 9,
  "blogPostId": 1
}
```

### Delete Like

```
DELETE /api/like/{id}
```

**Description:** Removes a like from a blog post.

**Authentication Required:** Yes (Admin or RegisteredUser roles only, and only the user who created the like or an admin can delete it)

**Parameters:**

- `id` (path parameter): The ID of the like to delete

**Response:** 204 No Content on success

## Users

### Register User

```
POST /api/user/register
```

**Description:** Registers a new user.

**Authentication Required:** No

**Request Body:**

```json
{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "Password123!"
}
```

**Response Example:**

```json
{
  "userId": 14,
  "username": "newuser",
  "email": "newuser@example.com",
  "role": 2,
  "blogPosts": [],
  "comments": [],
  "likes": []
}
```

### Login

```
POST /api/user/login
```

**Description:** Authenticates a user and returns a JWT token.

**Authentication Required:** No

**Request Body:**

```json
{
  "email": "newuser@example.com",
  "password": "Password123!"
}
```

**Response Example:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxNCIsInJvbGUiOiJSZWFkZXIiLCJuYmYiOjE3NDA5MjAyNTYsImV4cCI6MTc0MDkyNzQ1NiwiaWF0IjoxNzQwOTIwMjU2LCJpc3MiOiJHb3RoYW1Qb3N0QmxvZ0FQSSIsImF1ZCI6IkdvdGhhbVBvc3RCbG9nQVBJIn0.VtfXwNY9YeC1OF7NX61j4XBv003Ivs1YjxdGFeyGdWI"
}
```

### Get All Users

```
GET /api/user
```

**Description:** Retrieves all users.

**Authentication Required:** Yes (Admin role only)

**Response Example:**

```json
[
  {
    "userId": 8,
    "username": "adminuser",
    "email": "admin@example.com",
    "role": 0,
    "blogPosts": [
      {
        "blogPostId": 1,
        "title": "The Batman Strikes Again: Joker's Henchmen Captured!",
        "content": "Late last night, Gotham's vigilante took down an entire gang of Joker's men...",
        "dateCreated": "2025-02-26T23:23:48.726143",
        "userId": 8,
        "categoryId": 1
      }
    ],
    "comments": [],
    "likes": [
      {
        "likeId": 1,
        "userId": 8,
        "blogPostId": 1
      }
    ]
  },
  {
    "userId": 9,
    "username": "Joker",
    "email": "joker@gotham.com",
    "role": 1,
    "blogPosts": [],
    "comments": [
      {
        "commentId": 1,
        "commentContent": "Oh Batsy, always ruining my fun! But I must say... impressive work!",
        "dateCreated": "2025-02-26T23:42:45.413496",
        "userId": 9,
        "blogPostId": 1
      }
    ],
    "likes": []
  }
]
```

### Get User by ID

```
GET /api/user/{id}
```

**Description:** Retrieves a specific user by ID.

**Authentication Required:** Yes (Admin role or the user themselves)

**Parameters:**

- `id` (path parameter): The ID of the user to retrieve

**Response Example:**

```json
{
  "userId": 14,
  "username": "newuser",
  "email": "newuser@example.com",
  "role": 2,
  "blogPosts": [],
  "comments": [],
  "likes": []
}
```

### Update User

```
PUT /api/user/{id}
```

**Description:** Updates a user's profile.

**Authentication Required:** Yes (Admin role or the user themselves)

**Parameters:**

- `id` (path parameter): The ID of the user to update

**Request Body:**

```json
{
  "username": "updatedusername",
  "email": "updated@example.com",
  "password": "NewPassword123!"
}
```

**Response:** 204 No Content on success

### Update User Role

```
PUT /api/user/{id}/role
```

**Description:** Updates a user's role.

**Authentication Required:** Yes (Admin role only)

**Parameters:**

- `id` (path parameter): The ID of the user whose role to update

**Request Body:**

```json
{
  "newRole": "RegisteredUser"
}
```

**Response:** 204 No Content on success

### Delete User

```
DELETE /api/user/{id}
```

**Description:** Deletes a user.

**Authentication Required:** Yes (Admin role only)

**Parameters:**

- `id` (path parameter): The ID of the user to delete

**Response:** 204 No Content on success

## Role-Based Access Control

The API implements role-based access control with the following roles:

### Admin (Role 0)

- Has full access to all endpoints
- Can create, read, update, and delete all resources
- Can manage user roles

### RegisteredUser (Role 1)

- Can create blog posts, comments, and likes
- Can update and delete their own blog posts and comments
- Cannot delete other users' content
- Cannot manage user roles

### Reader (Role 2)

- Can read all public content
- Can add comments and likes to blog posts
- Can remove their own likes
- Cannot create, update, or delete blog posts
- Cannot update or delete comments
- Cannot manage user roles

## Rate Limiting

The API implements rate limiting to prevent abuse. The following headers are included in responses:

- `X-Rate-Limit-Limit`: The rate limit period (e.g., "1m" for 1 minute)
- `X-Rate-Limit-Remaining`: The number of requests remaining in the current window
- `X-Rate-Limit-Reset`: The time when the current rate limit window resets

## Error Responses

### 400 Bad Request

Returned when the request is malformed or contains invalid data.

### 401 Unauthorized

Returned when authentication is required but not provided or is invalid.

### 403 Forbidden

Returned when the authenticated user does not have permission to access the requested resource.

### 404 Not Found

Returned when the requested resource does not exist.

### 429 Too Many Requests

Returned when the rate limit has been exceeded.

### 500 Internal Server Error

Returned when an unexpected error occurs on the server.
