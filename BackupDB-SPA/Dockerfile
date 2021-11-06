FROM node:14-alpine AS build
WORKDIR /usr/src/app
COPY package*.json ./
RUN npm install &&  npm install -g @angular/cli 
RUN ng build --configuration=production --output-path=dist
COPY . .
EXPOSE 4200
# RUN chown -R node /usr/src/app
# USER node
# CMD ["npm", "start"]

### STAGE 2: Run ###
FROM nginx:1.17.1-alpine
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build /usr/src/app/dist /usr/share/nginx/html
