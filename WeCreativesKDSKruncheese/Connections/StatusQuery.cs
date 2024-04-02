using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeCreatives_KDSPJ.Connections
{
    public class StatusQuery
    {
        public string average_make_time = @"SELECT TO_CHAR(TRUNC(AVG(bumping_time) / 60), 'FM00') || ':' || TO_CHAR(MOD(AVG(bumping_time), 60), 'FM00') AS avg_bumping_time  FROM     kds WHERE snum = 31 and opendate = (select max(opendate)from kds ) and BUMPING_TIME is not null ";
        public string average_rack_time = @"WITH TimeData AS (
    SELECT 
        CASE 
            WHEN (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))) < INTERVAL '0' SECOND
                THEN '-' || TO_CHAR(ABS(EXTRACT(MINUTE FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))))) || ':' ||
                LPAD(ABS(EXTRACT(SECOND FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))))), 2, '0'))
            ELSE 
                TO_CHAR(EXTRACT(MINUTE FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))))) || ':' ||
                LPAD(EXTRACT(SECOND FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS'))))), 2, '0')
        END AS Rack_time 
    FROM 
        (SELECT 
            TO_CHAR(SYSDATE, 'DD-MON-YYYY') || ' ' || 
            TO_CHAR(TRUNC(TO_NUMBER(KDS.BUMPING_TIME)/3600), 'FM00') || ':' || 
            TO_CHAR(TRUNC(MOD(TO_NUMBER(KDS.BUMPING_TIME), 3600)/60), 'FM00') || ':' ||  
            TO_CHAR(MOD(TO_NUMBER(KDS.BUMPING_TIME), 60), 'FM00') AS combined_Time, 
            kdstime,
            TRANSACT
        FROM 
            KDS
        WHERE  
            KDS.SNUM = 31 AND OPENDATE=(Select Max(opendate) from kds) and BUMPING_TIME is not null) kds_bumped
    JOIN 
        POSHDELIVERY ph ON kds_bumped.TRANSACT = ph.TRANSACT
)

SELECT 
    CASE 
        WHEN ROUND(AVG(TO_NUMBER(SUBSTR(Rack_time, 1, INSTR(Rack_time, ':') - 1)) * 60 + 
            TO_NUMBER(SUBSTR(Rack_time, INSTR(Rack_time, ':') + 1))) - 270) < 59 THEN
            '00:' || LPAD(ROUND(AVG(TO_NUMBER(SUBSTR(Rack_time, 1, INSTR(Rack_time, ':') - 1)) * 60 + 
            TO_NUMBER(SUBSTR(Rack_time, INSTR(Rack_time, ':') + 1))) - 270), 2, '00')
        ELSE
            TO_CHAR(FLOOR(ROUND(AVG(TO_NUMBER(SUBSTR(Rack_time, 1, INSTR(Rack_time, ':') - 1)) * 60 + 
            TO_NUMBER(SUBSTR(Rack_time, INSTR(Rack_time, ':') + 1)))) / 60)) || ':' ||
            LPAD(FLOOR(MOD(ROUND(AVG(TO_NUMBER(SUBSTR(Rack_time, 1, INSTR(Rack_time, ':') - 1)) * 60 + 
            TO_NUMBER(SUBSTR(Rack_time, INSTR(Rack_time, ':') + 1)))), 60)), 2, '0')
    END AS Average_Rack_time 
FROM 
    TimeData";
        public string average_otd_time = @"WITH TimeData AS (
    SELECT 
        CASE 
            WHEN (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))) < INTERVAL '0' SECOND
                THEN '-' || TO_CHAR(ABS(EXTRACT(MINUTE FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP( combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))))) || ':' ||
                LPAD(ABS(EXTRACT(SECOND FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))))), 2, '0'))
            ELSE 
                TO_CHAR(EXTRACT(MINUTE FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))))) || ':' ||
                LPAD(EXTRACT(SECOND FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS'))))), 2, '0')
        END AS Rack_time, 
        kds_bumped.TRANSACT
    FROM (
        SELECT 
            TO_CHAR(SYSDATE, 'DD-MON-YYYY') || ' ' || 
            TO_CHAR(TRUNC(TO_NUMBER(KDS.BUMPING_TIME)/3600), 'FM00') || ':' ||  
            TO_CHAR(TRUNC(MOD(TO_NUMBER(KDS.BUMPING_TIME), 3600)/60), 'FM00') || ':' ||  
            TO_CHAR(MOD(TO_NUMBER(KDS.BUMPING_TIME), 60), 'FM00') AS combined_Time, 
            kdstime,
            TRANSACT
        FROM 
            KDS
        WHERE  
            KDS.SNUM = 31 AND OPENDATE =(Select Max(opendate) from kds) and BUMPING_TIME is not null
    ) kds_bumped
    JOIN 
        POSHDELIVERY ph ON kds_bumped.TRANSACT = ph.TRANSACT
)
SELECT 
    TO_CHAR(FLOOR(AVG(TO_NUMBER(SUBSTR(Rack_time, 1, INSTR(Rack_time, ':') - 1)) * 60 + TO_NUMBER(SUBSTR(Rack_time, INSTR(Rack_time, ':') + 1))) / 60)) || ':' ||
    LPAD(FLOOR(MOD(AVG(TO_NUMBER(SUBSTR(Rack_time, 1, INSTR(Rack_time, ':') - 1)) * 60 + TO_NUMBER(SUBSTR(Rack_time, INSTR(Rack_time, ':') + 1))), 60)), 2, '0') AS Average_OTD_time 
FROM 
    TimeData

";
        public string average_ttdt_time = @" WITH TimeData AS (
    SELECT 
        CASE 
            WHEN (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))) < INTERVAL '0' SECOND
                THEN '-' || TO_CHAR(ABS(EXTRACT(MINUTE FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))))) || ':' ||
                LPAD(ABS(EXTRACT(SECOND FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))))), 2, '0'))
            ELSE 
                TO_CHAR(EXTRACT(MINUTE FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))))) || ':' ||
                LPAD(EXTRACT(SECOND FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS'))))), 2, '0')
        END AS Rack_time, 
        kds_bumped.TRANSACT,  EXTRACT(MINUTE FROM ((timein - timeout)/2))*60 + EXTRACT(SECOND FROM ((timein - timeout)/2)) AS timer_in_seconds   ,timeout
    FROM (
        SELECT 
            TO_CHAR(SYSDATE, 'DD-MON-YYYY') || ' ' || 
            TO_CHAR(TRUNC(TO_NUMBER(KDS.BUMPING_TIME)/3600), 'FM00') || ':' || 
            TO_CHAR(TRUNC(MOD(TO_NUMBER(KDS.BUMPING_TIME), 3600)/60), 'FM00') || ':' || 
            TO_CHAR(MOD(TO_NUMBER(KDS.BUMPING_TIME), 60), 'FM00') AS combined_Time,  
            kdstime,
            TRANSACT
        FROM 
            KDS
        WHERE  
            KDS.SNUM = 31 AND OPENDATE =(select max(opendate) from kds) and BUMPING_TIME is not null 
    ) kds_bumped
    JOIN 
        POSHDELIVERY ph ON kds_bumped.TRANSACT = ph.TRANSACT AND TIMEIN IS NOT NULL
)
SELECT 
    TO_CHAR(TRUNC(AVG(timer_in_seconds + ((TO_NUMBER(SUBSTR(rack_time, 1, INSTR(rack_time, ':') - 1)) * 60) + TO_NUMBER(SUBSTR(rack_time, INSTR(rack_time, ':') + 1)))) / 60), 'FM00') || ':' ||
    LPAD(MOD(TRUNC(AVG(timer_in_seconds + ((TO_NUMBER(SUBSTR(rack_time, 1, INSTR(rack_time, ':') - 1)) * 60) + TO_NUMBER(SUBSTR(rack_time, INSTR(rack_time, ':') + 1))))), 60), 2, '0') AS average_ttdt_time
FROM 
    TimeData";

        public string average_CSC_percentage = @"  WITH TimeData AS ( SELECT  CASE    WHEN TO_NUMBER(SUBSTR(Rack_time, 1, INSTR(Rack_time, ':') - 1)) * 60 + TO_NUMBER(SUBSTR(Rack_time, INSTR(Rack_time, ':') + 1)) + 480 <= 2100 THEN (1 / COUNT(TRANSACT)) * 100  ELSE NULL        END AS CSC_percentage FROM ( SELECT  CASE  WHEN (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))) < INTERVAL '0' SECOND    THEN '-' || TO_CHAR(ABS(EXTRACT(MINUTE FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))))) || ':' ||  LPAD(ABS(EXTRACT(SECOND FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))))), 2, '0'))  ELSE  TO_CHAR(EXTRACT(MINUTE FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')))))) || ':' ||  LPAD(EXTRACT(SECOND FROM (ph.timeout - (kdstime + INTERVAL '1' MINUTE * EXTRACT(MINUTE FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS')) + INTERVAL '1' SECOND * EXTRACT(SECOND FROM TO_TIMESTAMP(combined_Time, 'DD-MON-YYYY HH24:MI:SS'))))), 2, '0')  END AS Rack_time,         kds_bumped.TRANSACT,         timeout  FROM (  SELECT  TO_CHAR(SYSDATE, 'DD-MON-YYYY') || ' ' ||    TO_CHAR(TRUNC(TO_NUMBER(KDS.BUMPING_TIME)/3600), 'FM00') || ':' ||   TO_CHAR(TRUNC(MOD(TO_NUMBER(KDS.BUMPING_TIME), 3600)/60), 'FM00') || ':' ||  TO_CHAR(MOD(TO_NUMBER(KDS.BUMPING_TIME), 60), 'FM00') AS combined_Time,   kdstime, TRANSACT  FROM  KDS WHERE   KDS.SNUM = 31 AND OPENDATE =(select max(opendate) from kds) and BUMPING_TIME is not null ) kds_bumped JOIN  POSHDELIVERY ph ON kds_bumped.TRANSACT = ph.TRANSACT ) subquery  GROUP BY       timeout, Rack_time) SELECT ROUND(AVG(CSC_percentage), 2) AS Average_CSC_percentage  FROM TimeData";
    }
}
