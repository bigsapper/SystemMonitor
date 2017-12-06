# SystemMonitor

**Summary:** The Systems Monitor is designed to monitor designated resources. Currently the only supported resources are network directories for file activities and Microsoft Message Queues for message activity.

**Overview:** The Systems Monitor is configured as a Windows Service named “Systems Monitor Service”. The service can be Started, Stopped, Paused, etc. like any standard Windows Service via the Services node of the MMC. The designated directories and message queues are listed in a configuration XML called \<appname>.config. The resources being monitored by the service can be managed by adding and/or removing items in this file. Once the service is running, counters that show the data points can been seen via the Performance Counter node of the MMC, which can be accessed from any desktop which has this software and access to the network.

**Config File:** To access the config file look in the application’s installation directory. The config file is named \<appname>.config. The file is a simple text file formatted as an XML document. Load the file in a text editor such as Notepad. The configuration nodes are “AppSettings”, “WatchedMessageQueues” and “WatchedDirectories”. All the child nodes are named “add” and contain both a “key” attribute and a “value” attribute. The only supported app setting value is the time interval in seconds at which the service queries the designated resources for data points. For the watched message queues, the “key” attribute is a user-friendly name and should be unique within the parent node; the “value” attribute is the path to a Microsoft Message Queue. For the watched directories, the “key” attribute is a user-friendly name and should be unique within the parent node; the “value” attribute is a local or networked directory. UNC paths are supported. The value for any “key” attribute should **not** contain any forward slashes or back slashes. To add a resource, add an XML child node following the described pattern. To remove a resource, either physically delete the XML child node or rename the node to any value but “add”. *For the changes to take affect, the service must be restarted.*

**Performance Monitor:** The data point values collected by the service can be observed in the Performance Monitor MMC interface. To load the MMC node for the Window Services, click “Start”->”Run”. In the “Run” dialog box, type “perfmon.msc /s” and click “Ok”. The standard operation of the Performance Monitor MMC interface can be ascertained by perusing the help documentation accessable from the Help menu. After the MMC app loads, click the toolbar button with the plus(“+”) sign icon to add a counter:
* In the “Add Counters” dialog box, choose the “Select counters from computer” selection. 
* In the text field immediately below this selection enter the name of the server that this service is installed on and hit TAB. 
* In the dropdown list under “Performance object”, select the "Directory Watcher” list item. 
* Choose the “All Counters” selection. 
* Click the “Add” button to add the counters to the display graph.

Repeat for the "Message Queue Watcher” performance object. Click the “Close” button when finished. To see an explanation for each individual counter, choose the “Select counter from list” selection instead of the “All counters” slection. Click the “Explain” button and select a counter from the list. The explanation for that counter will be in the “Explain” dialog box. When finished, click the “Close” button.

Observe the data points reported by the service.
