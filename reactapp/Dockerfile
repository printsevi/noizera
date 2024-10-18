# build the app
FROM node:20 
# ENV NODE_ENV=production
WORKDIR /reactapp  
COPY package*.json ./  
RUN npm ci
COPY . .  
RUN npm run build  
EXPOSE 3000  
CMD ["npm", "start"]  


# Set a version
LABEL version="1.0.0"