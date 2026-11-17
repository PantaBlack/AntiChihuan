## Stage 1: Compilar Frontend Angular 18
FROM node:20-alpine AS frontend-build
WORKDIR /frontend
COPY frontend/package*.json ./
RUN npm install
COPY frontend/ .
RUN npm run build

## Stage 2: Empaquetar Frontend en Spring Boot 3 y compilar JAR con Maven (JDK 21)
FROM maven:3.9.9-eclipse-temurin-21 AS backend-build
WORKDIR /backend
COPY backend/pom.xml .
RUN mvn dependency:go-offline -B
COPY backend/src ./src
COPY --from=frontend-build /frontend/dist/antichihuan-angular-web/browser/ ./src/main/resources/static/
RUN mvn clean package -DskipTests

## Stage 3: Imagen de Producción Ligera (Eclipse Temurin JRE 21)
FROM eclipse-temurin:21-jre-alpine AS runtime
WORKDIR /app
COPY --from=backend-build /backend/target/antichihuan-api-1.0.0.jar app.jar

ENV SERVER_PORT=8080
ENV SPRING_PROFILES_ACTIVE=prod
EXPOSE 8080

ENTRYPOINT ["java", "-jar", "app.jar"]
