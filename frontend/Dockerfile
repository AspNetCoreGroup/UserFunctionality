FROM node:alpine

WORKDIR /src
COPY ./ ./

RUN npm i
RUN npm run build
ENTRYPOINT [ "npm", "start" ]
