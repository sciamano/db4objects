#!/bin/bash
export VMEXE=java
$VMEXE -Djava.library.path=lib/linux -classpath lib/db4o-4.5-java1.4.jar:lib/kxml-plugin.jar:lib/osgi_core.jar:lib/xmlpull_1_1_3_1.jar:lib/jakarta-oro-2.0.7.jar:lib/runtime.jar:lib/xswt.jar:lib/jface.jar:lib/objectmanager.jar:lib/linux/swt-cairo.jar:lib/linux/swt.jar:lib/linux/swt-mozilla.jar:lib/linux/swt-pi.jar com.db4o.browser.gui.standalone.StandaloneBrowser

