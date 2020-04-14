docker run --rm \
  -p 443:443 -p 80:80 --name letsencrypt \
  -v "/etc/letsencrypt:/etc/letsencrypt" \
  -v "/var/lib/letsencrypt:/var/lib/letsencrypt" \
  certbot/certbot certonly -n \
  -m "michael@appspark.at" \
  -d intern.s-gartl.at \
  --standalone --agree-tos