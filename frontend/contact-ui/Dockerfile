# # Build Stage
# FROM node:18-alpine AS build
# WORKDIR /app
# COPY package*.json ./
# RUN npm install
# COPY . .
# RUN npm run build

# # Production Stage
# FROM nginx:stable-alpine AS production
# COPY --from=build /app/build /usr/share/nginx/html
# EXPOSE 80
# CMD ["nginx", "-g", "daemon off;"]


# ===========================
# BUILD STAGE
# ===========================
FROM node:20-alpine AS build

WORKDIR /app

# Install dependencies with caching
COPY package*.json ./
RUN npm ci

# Copy source
COPY . .

# Build optimized production bundle
RUN npm run build

# ===========================
# RUNTIME STAGE
# ===========================
FROM nginx:1.25-alpine

# Remove default nginx config
RUN rm /etc/nginx/conf.d/default.conf

# Copy custom nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Fix permissions for non-root execution
RUN mkdir -p /var/cache/nginx \
    && chmod -R 777 /var/cache/nginx \
    && chmod -R 777 /var/run \
    && chmod -R 777 /usr/share/nginx/html

# Copy build output
COPY --from=build /app/dist /usr/share/nginx/html

# Security: run as non-root
RUN adduser -D frontend && chown -R frontend /usr/share/nginx/html
USER frontend

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=3s \
    CMD wget --spider -q http://localhost || exit 1

CMD ["nginx", "-g", "daemon off;"]
