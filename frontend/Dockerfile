## Stage 1: Compilación Angular 18 con Node 20
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

## Stage 2: Servidor Web Nginx para Producción
FROM nginx:alpine AS runtime
COPY --from=build /app/dist/antichihuan-angular-web/browser /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
