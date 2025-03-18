# Build My Frontend Framework
FROM node:lts-alpine AS builder
# is app folder inside of image
WORKDIR /app
# is app folder in project
# I will copy the content of my app folder to inside image
COPY ./app .

FROM nginx:alpine
# I will copy the content from builder stage to nginx public folder
COPY --from=builder /app /usr/share/nginx/html
COPY default.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]