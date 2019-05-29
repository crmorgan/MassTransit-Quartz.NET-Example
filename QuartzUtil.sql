select top 1 * from QRTZ_JOB_DETAILS
select * from QRTZ_TRIGGERS
select * from QRTZ_SIMPLE_TRIGGERS
select top 1 * from QRTZ_LOCKS

select count(*) from QRTZ_JOB_DETAILS
select count(*) from QRTZ_TRIGGERS
select count(*) from QRTZ_SIMPLE_TRIGGERS
select count(*) from QRTZ_PAUSED_TRIGGER_GRPS
select count(*) from QRTZ_FIRED_TRIGGERS
select count(*) from QRTZ_LOCKS
--exec sp_who2


update QRTZ_TRIGGERS set TRIGGER_STATE = 'WAITING', NEXT_FIRE_TIME = 636937197657316850 where TRIGGER_STATE = 'ERROR'