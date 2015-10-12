# Nant #

If you would like to integrate the database schema renew/update with nant it can easily be done by adding the following to your nant build

The following target will initialize the database using the **UAT** profile and the dbupdate.xml file

```
<target name="dvc-initialize-update">
  <echo message="Initialising"/>
  <property name="dvc.folder" value="var\database\"/>
  <property name="dvc.executable" value="dvc.exe"/>
  <exec program="${dvc.executable}" basedir="${dvc.folder}" workingdir="${dvc.folder}" >
    <arg value="-q"/>
    <arg value="-iu"/>
    <arg value="-p=&quot;UAT&quot;"/>
    <arg value="-f=&quot;dbupdate.xml&quot;" />
  </exec>
</target>
```

The following target will rollback the database initialization using the **UAT** profile and the dbupdate.xml file
```
<target name="dvc-initialize-rollback">
  <echo message="Initialising"/>
  <property name="dvc.folder" value="var\database\"/>
  <property name="dvc.executable" value="dvc.exe"/>
  <exec program="${dvc.executable}" basedir="${dvc.folder}" workingdir="${dvc.folder}" failonerror="false"  >
    <arg value="-q"/>
    <arg value="--ir"/>
    <arg value="-p=&quot;UAT&quot;"/>
    <arg value="-f=&quot;dbupdate.xml&quot;" />
  </exec>
</target>
```