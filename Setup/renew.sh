docker run --rm --name letsencrypt \
    -v "/etc/letsencrypt:/etc/letsencrypt" \
    -v "/var/lib/letsencrypt:/var/lib/letsencrypt" \
    -v "/usr/share/nginx/html:/var/www/certbot" \
    certbot/certbot:latest \
    --webroot -w /var/www/certbot \
    renew --quiet

# Show certs:
#docker run --rm --name letsencrypt \
#    -v "/etc/letsencrypt:/etc/letsencrypt" \
#    -v "/var/lib/letsencrypt:/var/lib/letsencrypt" \
#    -v "/usr/share/nginx/html:/var/www/certbot" \
#    certbot/certbot:latest \
#    certificates