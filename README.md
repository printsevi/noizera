# Noizera

1. Create Droplet

2. Run console 

3. Setup Ubuntu https://www.digitalocean.com/community/tutorials/initial-server-setup-with-ubuntu

4. setup nginx https://www.digitalocean.com/community/tutorials/how-to-install-nginx-on-ubuntu-20-04

5. firewall https://www.digitalocean.com/community/tutorials/ufw-essentials-common-firewall-rules-and-commands

4. Install Docker on the droplet https://www.digitalocean.com/community/tutorials/how-to-install-and-use-docker-on-ubuntu-22-04

to remove containers: sudo systemctl restart docker.socket docker.service; sudo docker rm $(sudo docker ps -a -q) -f

5. Install docker-compose https://www.digitalocean.com/community/tutorials/how-to-install-and-use-docker-compose-on-ubuntu-22-04

5. Using PUtty and conversion create a file with private OpenSSH code and add it to Action Secrets

7. Add ssh public key to droplet using command "nano ~/.ssh/authorized_keys"

8. A possible fix of auth error would be to add the following to your sshd_config file. https://github.com/appleboy/scp-action

sudo nano /etc/ssh/sshd_config

CASignatureAlgorithms +ssh-rsa
HostKeyAlgorithms +ssh-rsa
PubkeyAcceptedKeyTypes +ssh-rsa
PubkeyAuthentication yes
PasswordAuthentication no

sudo systemctl restart ssh


9. Setup GitHub actions in .github folder

10. Add Actions secrets (token from Digital Ocean API, ssh keys etc)

11. via RDP setup auth

docker login
sudo groupadd docker
sudo usermod -aG docker $USER
newgrp docker

12. http://172.18.0.2:3000/ check this by default
check docker "sudo systemctl status docker"

13. generate new token in dockerhub and login in droplet

14. update nginx after docker compose pull https://gist.github.com/hlubek/02955a3f28db168417884b5397ce07c0

sudo nano /etc/nginx/sites-available/noizera

sudo nginx -t
sudo systemctl restart nginx

15. Config for API

proxy_cache_path /var/cache/nginx levels=1:2 keys_zone=STATIC:10m inactive=7d use_temp_path=off;

upstream nextjs_upstream {
  server localhost:3000;
}

upstream imgproxy_upstream {
  server localhost:8080;
}

server {
  listen 80;

  server_name noizera-admin.com;

  server_tokens off;

  gzip on;
  gzip_proxied any;
  gzip_comp_level 4;
  gzip_types text/css application/javascript image/svg+xml;

  proxy_http_version 1.1;
  proxy_set_header Upgrade $http_upgrade;
  proxy_set_header Connection 'upgrade';
  proxy_set_header Host $host;
  proxy_cache_bypass $http_upgrade;

  merge_slashes off;

  location /img/ {

    proxy_cache STATIC;

    proxy_pass http://imgproxy_upstream/;

    # For testing cache - remove before deploying to production
    add_header X-Cache-Status $upstream_cache_status;
  }

  location /_next/static {
    proxy_cache STATIC;
    proxy_pass http://nextjs_upstream;

    # For testing cache - remove before deploying to production
    add_header X-Cache-Status $upstream_cache_status;
  }

  location /static {
    proxy_cache STATIC;

    # Ignore cache control for Next.js assets from /static, re-validate after 60m
    proxy_ignore_headers Cache-Control;
    proxy_cache_valid 60m;

    proxy_pass http://nextjs_upstream;

    # For testing cache - remove before deploying to production
    add_header X-Cache-Status $upstream_cache_status;
  }

  location / {
    proxy_pass http://nextjs_upstream;
  }

  location /api/ {
        proxy_pass http://localhost:5000/api/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
  }

  location /seq/
 {
        proxy_pass http://localhost:8081/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
}
}

16. Generate password in bash for i in {1..10}; do (tr -cd '[:alnum:]' < /dev/urandom | fold -w 30 | head -n 1); done

17. use owner of the db username

18. create local certificates. Open terminal in a folder.

sudo apt update
sudo apt install openssl
openssl genpkey -algorithm RSA -out aspnetcore.key -aes256
openssl req -new -key aspnetcore.key -out aspnetcore.csr
openssl x509 -req -days 365 -in aspnetcore.csr -signkey aspnetcore.key -out aspnetcore.crt
openssl pkcs12 -export -out aspnetcore.pfx -inkey aspnetcore.key -in aspnetcore.crt

- Open Chrome and go to chrome://settings/security.
- Scroll down to the "Manage certificates" section and click on it.
- In the "Certificates" window, click on the "Authorities" tab.
- Click on "Import" and select the self-signed certificate file (my-local.dev.crt).
- Follow the prompts to import the certificate. Ensure you select the option to "Trust this certificate for identifying websites."

19. create public certificate https://www.inmotionhosting.com/support/website/ssl/lets-encrypt-ssl-ubuntu-with-certbot/

20. Subscriptions 


INSERT INTO public."Subscriptions" (
    "Id", "Title", "Price", "IsDisabled", "ProfileTypes", "StripePriceId",
    "FreeTrialInDays", "IsAnnual", "RoyaltyShare", "SubscriptionType"
) VALUES (
    gen_random_uuid(), -- Auto-generate a new UUID for the Id
    'Noizera Premium', -- Replace with the title of the plan
    7.99, -- Replace with the plan price
    FALSE, -- Replace with true if the plan is disabled
    'Fan,Artist,Label', -- Comma-separated profile types
    'price_1QIDRBDUaPTgFsFqAFlcvs2b', -- Replace with the Stripe price ID
    7, -- Replace with the free trial period in days
    FALSE, -- Replace with true for annual plans
    0.7, -- Replace with the royalty share percentage
    'PremiumListeningWithFreeTrial' -- Replace with the type of subscription
);

INSERT INTO public."Subscriptions" (
    "Id", "Title", "Price", "IsDisabled", "ProfileTypes", "StripePriceId",
    "FreeTrialInDays", "IsAnnual", "RoyaltyShare", "SubscriptionType"
) VALUES (
    gen_random_uuid(), -- Auto-generate a new UUID for the Id
    'Noizera Premium', -- Replace with the title of the plan
    69.99, -- Replace with the plan price
    FALSE, -- Replace with true if the plan is disabled
    'Fan,Artist,Label', -- Comma-separated profile types
    'price_1QIDRBDUaPTgFsFqCzZ6T7WV', -- Replace with the Stripe price ID
    14, -- Replace with the free trial period in days
    TRUE, -- Replace with true for annual plans
    0.7, -- Replace with the royalty share percentage
    'PremiumListeningWithFreeTrial' -- Replace with the type of subscription
);

21. Terms

INSERT INTO public."Terms" ("Id", "EffectiveDate", "Content")
VALUES 
    (gen_random_uuid(), '2024-12-16', 'Terms and conditions for December 2024.');