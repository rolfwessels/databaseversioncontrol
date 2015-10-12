# How does it work #

When the database gets created the DVC system will add an additional table. This table will contain a record stating at which "index" the current database is. When you run _dvc.exe -u_  the dvc system will run updates in order but only updates which have an index that is greater than the index stored in the tracking table. All updates are processed as one transaction so if one fails all of them will fail and roll-back. Once the updates are complete the index is updated in the tracking table.

The tracking table is configured in the profile and looks something like this
```
<trackingByTable tableName="DVC_Tracking" autoCreate="True" />
```

  * **tableName** - The name of the table that will be created
  * **autoCreate** - Means the table will automatically be created when we run dvc initialize.

