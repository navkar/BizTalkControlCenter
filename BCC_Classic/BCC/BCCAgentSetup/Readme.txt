==============================================
MOD NAME---> BizTalk Control Center (BCC) Agent 
VERSION----> 1.0
CREATED BY-> Naveen Karamchetti
EMAIL : biztalkcontrolcenter@gmail.com
==============================================

INDEX:

-> Requirements
-> Installation
-> Configuration
-> Known Bugs & Issues
-> Version History
-> Incompatibilities & Save game warnings
-> Credits & Usage

==============================================
REQUIREMENTS:
==============================================
This is Windows Service which runs on the same box as of BizTalk Server.

===============================================
INSTALLATION:
===============================================
Run the setup.exe and install it to the default location. 
The service will automatically start after installation.

===============================================
CONFIGURATION:
===============================================
You can configure the Agent email settings using Speedcode '603'.

Email Host : smtp.gmail.host
Email Port : 587
From email ID: biztalkcontrolcenter@gmail.com 
Password : P@ssw0rd$
Recipient Email id: (Your email id) 
SSL Enabled : True ( True: For Web-based email, False: For intranet/exchange servers)

===============================================
KNOWN ISSUES OR BUGS:
===============================================
The Agent takes about 60 seconds to register all the BizTalk artifacts. 

===============================================
VERSION HISTORY
===============================================
1.0 - First version (very stable)

===============================================
CREDITS & USAGE:
===============================================
Naveen Karamchetti. (biztalkcontrolcenter@gmail.com)