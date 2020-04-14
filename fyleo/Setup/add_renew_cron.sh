#write out current crontab
crontab -l > fyleosslcron
#echo new cron into cron file
echo "19 0,12 * * * renew.sh" >> fyleosslcron
#install new cron file
crontab fyleosslcron
#rm fyleosslcron