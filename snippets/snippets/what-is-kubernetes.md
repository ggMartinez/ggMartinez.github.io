---
sidebar_position: 1
---

# Docker Compose Template

```
version: '2.2'
services:
  container1:
    image: image1
    container_name: container1
    tty: true
    restart: unless-stopped
    ports:
      - "443:443"
      - "80:80"
   build:
      context: ./
      dockerfile: dockerfile
    environment:
      - var=value
    volumes:
      - /dirHost:/dirContainer
    extra_hosts:
      - "host:IP"
    networks:
      network_name:
        ipv4_address: 1.1.1.1
 
 
networks:
    network_name:
        driver: bridge
        driver_opts:
              com.docker.network.enable_ipv6: "false"
        ipam:
            driver: default
            config:
            - subnet: "1.1.1.0/24"
              gateway: "1.1.1.1"
```