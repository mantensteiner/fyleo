# Create folders
mkdir -p /var/lib/letsencrypt
mkdir -p /etc/letsencrypt
mkdir -p /usr/share/nginx/html

# Install Docker 
sudo apt update
sudo apt install apt-transport-https ca-certificates curl software-properties-common
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu bionic stable"
sudo apt update
apt-cache policy docker-ce
sudo apt install docker-ce
sudo systemctl status docker

# Init Docker swarm
docker swarm init --advertise-addr IP

# Copy files on server

# Generate ssl certificate
bash generate.sh

# Login to Docker-hub for pivate repo access
docker login --username=mantenpanther 

# Pull images (should not be necessary, but more stable with swarm)
docker pull nginx:stable-alpine
docker pull mantenpanther/fyleo_gartl-app:latest
docker pull mazzolino/shepherd

# Start stack 
docker stack deploy -c docker-compose.yml fyleo_gartl

# Create cron-job for ssl-renewal
bash add_renew_cron.sh