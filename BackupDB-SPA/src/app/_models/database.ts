export interface Database {
    database_name : string ,
    backup_start_date: Date,
    backup_finish_date: Date,
    expiration_date: null,
    backup_type: string,
    backup_size: number,
    logical_device_name: string,
    physical_device_name: string,
    backupset_name: string,
    description: string
}
