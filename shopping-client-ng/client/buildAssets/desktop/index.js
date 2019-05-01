const { app, BrowserWindow, globalShortcut, Menu, Tray } = require('electron');
const appMenu = require('./appMenu');

const path = require('path');
const url = require('url');

let win;
let trayApp;

function createWindow() {
  win = new BrowserWindow({
    minWidth: 1080,
    minHeight: 600,
    width: 1080,
    height: 600
  });

  win.loadURL(url.format({
    pathname: path.join(__dirname, 'web/index.html'),
    protocol: 'file:',
    slashes: true
  }));

  buildTrayIcon();
  appMenu.buildMenu();

  globalShortcut.register('CmdOrCtrl+Shift+i', () => {
    win.webContents.toggleDevTools();
  });

  globalShortcut.register('CmdOrCtrl+Shift+b', function () {
    if (process.platform == 'darwin') {
      app.dock.bounce('critical');
    }
    app.dock.setBadge('EVIL:)');
  });

  win.on('closed', () => {
    win = null;
  });
}

app.on('ready', createWindow);

app.on('window-all-closed', () => {
  globalShortcut.unregisterAll();
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('activate', () => {
  if (win === null) {
    createWindow();
  }
});

let buildTrayIcon = () => {
  let trayIconPath = path.join(__dirname, 'icon.png');
  var contextMenu = Menu.buildFromTemplate([
    {
      label: 'Pokemons...',
      type: 'normal',
      click: function () {
        win.webContents.send('navigateTo', 'pokemon/list/pokemon/1');
      }
    },
    {
      label: 'Quit',
      accelerator: 'Command+Q',
      selector: 'terminate:'
    }
  ]);

  trayApp = new Tray(trayIconPath);
  trayApp.setToolTip('ng Demo');
  trayApp.setContextMenu(contextMenu);
};
