package com.db4o.objectManager.v2;

import com.jgoodies.looks.Options;
import com.db4o.objectManager.v2.util.Log;
import demo.objectmanager.model.DemoPopulator;

import javax.swing.*;
import java.awt.event.ActionListener;
import java.awt.event.ActionEvent;

/**
 * User: treeder
 * Date: Aug 30, 2006
 * Time: 12:27:13 PM
 */
public class DashboardMenuBar extends BaseMenuBar{
    private Dashboard dashboard;

    public DashboardMenuBar(Dashboard dashboard2) {
        super();
        this.dashboard = dashboard2;
        JMenu menu;
        JMenuItem item;
        this.putClientProperty(Options.HEADER_STYLE_KEY, Boolean.TRUE);

        menu = new JMenu("File");
        menu.add(new JMenuItem("New..."));
        menu.add(new JMenuItem("Open..."));
        menu.add(new JMenuItem("Save"));
        menu.addSeparator();
        menu.add(new JMenuItem("Print..."));
        menu.addSeparator();

        item = createMenuItem("Create Demo Db");
        item.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                try {
                    DemoPopulator demoPopulator = new DemoPopulator();
                    demoPopulator.start();
                    dashboard.connectTo(demoPopulator.getDataFile());
                } catch(Exception ex){
                    ex.printStackTrace();
                    dashboard.showError("Error creating model database! " + ex.getMessage());
                }
            }
        });
        menu.add(item);
        this.add(menu);

        menu = new JMenu("Edit");
        menu.add(new JMenuItem("Cut"));
        menu.add(new JMenuItem("Copy"));
        menu.add(new JMenuItem("Paste"));
        this.add(menu);

        menu = new JMenu("Help");
        menu.add(new JMenuItem("About"));
        menu.addSeparator();
        item = new JMenuItem("Exception Log");
        item.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                JFrame logFrame = new JFrame("Exception Log");
                JTextArea ta = new JTextArea(20, 80);
                JScrollPane scrollPane = new JScrollPane(ta);
                logFrame.add(scrollPane);
                //logFrame.setSize(400, 300);
                logFrame.pack();
                logFrame.setVisible(true);
                ta.setText(Log.dump());
            }
        });
        menu.add(item);
        this.add(menu);

    }
}
