SELECT 
 Nickname,
 Max(CreateTime) AS 'CreateTime',
 COUNT(1) AS 'StoreCount'
FROM ORDERINFO O JOIN CONSUMERINFO C
ON O.ConsumerID=C.ID
GROUP BY Nickname,HuodongID
ORDER BY StoreCount DESC