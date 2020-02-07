# Setup a new node/swarm cluster

1. Fire up a new server with ssh-access and open ports 80 and 443
   
2. Install Docker
   
3. Setup Docker for Swarm-Mode
   docker swarm init --advertise-addr IP

4. Create/copy following files on server (e.g. /home/fyleo): setup.sh,generate.sh,..
   scp -r ./Setup root@IP:/home/fyleo

5. Execute setup.sh to create necessary shared folders on host (for challenge-files, certificates,...)

6. Execute generate.sh to obtain certificate from letsencrypt
   Based on reference (1).

7. Create/copy nginx.conf and docker-compose.yml
   docker stack deploy -c docker-compose.yml fyleo
   => Stack with swarm-services starts.
   Destroy stack with "docker stack rm fyleo" if something goes wrong.

8.  Create/copy renew.sh and add_renew_cron.sh.
    Execute add_renew_cron.sh to register a cron-job which executes renew.sh for auto-renewal of certificates.

# Manual Backup
scp -r root@IP:/home/fyleo ./fyleo_backup
