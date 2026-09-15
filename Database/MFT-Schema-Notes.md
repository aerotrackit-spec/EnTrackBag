# Verified schema notes from MFT Script Latest.txt

The supplied 14/08/2026 script confirms Readers.ID, Antennas.ID, LogicalDevice.ID, LogicalDeviceMap.ID, ReaderControllerMap.TID, SuspectBags.ID, and SystemSettings.ID as primary keys. Antennas contains ReaderID and AntennaPort; LogicalDeviceMap contains LogicalDeviceID, ReaderID, AntennaID; Readers contains ReaderCode, ReaderLocation, ReaderIP, ReaderPort, LastConnected, LastDisconnected, LastError, Status and ReaderHostName. SuspectBags contains TagID, TStamp, StationID, RecheckStationID, RecheckTime, LastStage, ExitGateID, LastSeenTime and LastSeenAt.

Do not infer physical antenna count from reader ports.
