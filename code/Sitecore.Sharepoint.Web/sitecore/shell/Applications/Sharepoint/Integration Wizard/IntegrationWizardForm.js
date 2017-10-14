var animation =
{
    onExpand       : null,
    onCollapse     : null,
    onCrossFade    : null,
    onFadeIn       : null,
    onFadeOut      : null,
    onChainComplete: null,

    fn_array: null,

    chain: function(arr)
    {
        if (arr)
        {
            if (this.fn_array)
            {
                this.fn_array.splice();
                delete this.fn_array;
            }

            this.fn_array = arr;
        }

        if (this.fn_array && this.fn_array[0]) {
            this.fn_array.shift()();
        }

        else
        {
            if (this.onChainComplete) {
                this.onChainComplete();
            }
        }
    },

    collapse: function(element)
    {
        return function()
        {
            var fx = new FX.Element(element);
                fx.setOptions({duration: 200}).animate({height: 0});
                fx.onEnded(function()
                {
                    if (animation.onCollapse) {
                        animation.onCollapse();
                    }

                    animation.chain();
                });

            fx.play();
        }
    },

    expand: function(element, destHeight)
    {
        return function()
        {
            var children = element.childElements();

            var fx = new FX.Element(element);
                fx.setOptions({duration: 200}).animate({height: destHeight||48});
                fx.onEnded(function()
                {
                    if (animation.onExpand) {
                        animation.onExpand();
                    }

                    animation.chain();
                });

            fx.play();
        }
    },

    showChildren: function(element)
    {
        return function()
        {
            var children = element.childElements();

            children.each(function(obj) {
                obj.setStyle({display: 'inline-block', opacity: 1});
            })

            animation.chain();
        }
    },

    hideChildren: function(element)
    {
        return function()
        {
            var children = element.childElements();

            children.each(function(obj) {
                obj.setStyle({display: 'none', opacity: 0});
            });

            animation.chain();
        }
    },

    fadeOutChildren: function(element)
    {
        return function()
        {
            var children = element.childElements();

            var fx1 = new FX.Element(children[0]);
                fx1.setOptions({duration: 200}).animate({opacity: 0});
                fx1.onEnded(function() { children[0].hide(); });
                fx1.play();

            var fx2 = new FX.Element(children[1]);
                fx2.setOptions({duration: 200}).animate({opacity: 0});
                fx2.onEnded(function()
                {
                    children[1].hide();

                    if (animation.onFadeOut) {
                        animation.onFadeOut();
                    }

                    animation.chain();
                });

            fx2.play();
        }
    },

    fadeInChildren: function(element)
    {
        return function()
        {
            var children = element.childElements();

            children.each(function(obj) {
                obj.setStyle({display: 'block', opacity: 0});
            })

            var fx1 = new FX.Element(children[0]);
                fx1.setOptions({duration: 200}).animate({opacity: 1});
                fx1.play();

            var fx2 = new FX.Element(children[1]);
                fx2.setOptions({duration: 200}).animate({opacity: 1});
                fx2.onEnded(function()
                {
                    if (animation.onFadeIn) {
                        animation.onFadeIn();
                    }

                    animation.chain();
                });

            fx2.play();
        }
    },

    fadeIn: function(element)
    {
        return function()
        {
            element.setStyle({opacity: 0});

            var fx = new FX.Element(element);
                fx.setOptions({duration: 200}).animate({opacity: 1});
                fx.onEnded(function()
                {
                    if (animation.onFadeIn) {
                        animation.onFadeIn();
                    }

                    animation.chain();
                });

            fx.play();
        };
    },

    fadeOut: function(element)
    {
        return function()
        {
            element.setStyle({opacity: 1});

            var fx = new FX.Element(element);
                fx.setOptions({duration: 200}).animate({opacity: 0});
                fx.onEnded(function()
                {
                    if (animation.onFadeOut) {
                        animation.onFadeOut();
                    }

                    animation.chain();
                });

            fx.play();
        };
    },

    crossFade: function(element1, element2, callback)
    {
        return function()
        {
            var fx1 = new FX.Element(element1);
                fx1.setOptions({duration: 200}).animate({opacity: 0});
                fx1.onEnded(function()
                {
                    element1.hide();

                    element2.setStyle({opacity: 0});
                    element2.show();

                    var fx2 = new FX.Element(element2);
                        fx2.setOptions({duration: 200}).animate({opacity: 1});
                        fx2.play();
                });

            fx1.play();
        };
    }
}

function getPattern(serverUrl)
{
    //
    // pattern should looks something like this: /http:\/\/vs-intranet(?![^\/])/i
    // It means that pattern should match any string (server url) that is not followed by any symbol except '/'
    //

    var pattern = "^" + serverUrl.split("/").join("\\/");
        pattern += "(?![^\\/])";

    return new RegExp(pattern, 'i');
}

function getServerList()
{
    if (this.serverList == undefined)
    {
        this.serverList = [];

        var knownServersElement = document.getElementById("knownServers");
        var knownServers = knownServersElement.value;
        var servers = knownServers.split("|");

        for (var i=0; i<servers.length; i++)
        {
            var serverInfo = servers[i].split("^");

            serverList.push({
                userName : serverInfo[0],
                serverUrl: serverInfo[1],
                pattern: getPattern(serverInfo[1]),
                connectionConfiguration: serverInfo[2]
            });
        }
    }

    return this.serverList||[];
}

function UpdateInfo()
{
  window.userName = $("defaultUser").value;
  window.connectionConfiguration = $("defaultConfiguration").value;
  
  var serverList = getServerList();
  var clientUrl = $("server").value;
  var configFound = false;

  for (var i = 0; i < serverList.length; i++) {
    if (serverList[i].pattern.exec(clientUrl)) {
      window.userName = serverList[i].userName;
      if (serverList[i].connectionConfiguration) {
        window.connectionConfiguration = serverList[i].connectionConfiguration;
      }
      configFound = true;
      break;
    }
  }

  var altconf = $("alternativeCredentials").checked;
  
  if (altconf && $("isCredentials").checked) {
    $("currentUser").innerHTML = $("userName").value;
  } else {
    $("currentUser").innerHTML = window.userName;
  }
  

  if (altconf && $("isAdvanced").checked) {
    $("currentConfiguration").innerHTML = $("configuration").options[$("configuration").selectedIndex].text;
  } else {
    $("currentConfiguration").innerHTML = window.connectionConfiguration;
  }
  $("confirmConfiguration").innerHTML = $("currentConfiguration").innerHTML;
  
  if (configFound) {
    $("notPredefinedConfigurationInfo").hide();
    $("predefinedConfigurationInfo").show();
  }
  else
  {
    $("notPredefinedConfigurationInfo").show();
    $("predefinedConfigurationInfo").hide();
  }
}

function SwitchRadioButton(name, value) {
    $(name).checked = value;
}

function switchToDefCredentials()
{
  $('alternativeCredentialsBlock').hide();
  UpdateInfo();
}

function switchToAltCredentials()
{
  $('alternativeCredentialsBlock').show();
  UpdateInfo();
}

function SetDivVisibility(divId, visible) {
  $(divId).style.display = visible ? "" : "none";
  UpdateInfo();
}

function mappingContainerMouseOver(sender)
{
    sender.onmouseover = null

    var tooltip = $$(".tooltipForMappedField")[0];
    var timer   = null;
    var combos  = $$('.spField');

    for (var i=0; i<combos.length; i++)
    {
        combos[i].onmouseover = function(event)
        {
            var dropdown = this;

            timer = setTimeout(function()
            {
                $(tooltip).update(dropdown.value);
                tooltip.style.display = "block";
            },
            500);
        }

        combos[i].onmousemove = function(event)
        {
            var e = event||window.event;

            tooltip.style.left = e.clientX + "px";
            tooltip.style.top  = e.clientY - 28 + "px";
        }

        combos[i].onmouseout = function(event)
        {
            if (timer) {
                clearTimeout(timer);
            }

            tooltip.style.display = "none";
        }
    }
}

function mappingSelectorFocus()
{
    $('createNewMapping')  .checked = false;
    $('useExistentMapping').checked = true;

    showInfoPanel();

    return true;
}

function showInfoPanel()
{
    $$('.mappingsInfoLabel')[0].parentNode.style.display = "none";
    $('mappingInfoPanel').parentNode.parentNode.style.display = "block";
}

function createNewMappingRadioClick(sender)
{
    $$('.mappingsInfoLabel')[0].parentNode.style.display = "block";
    $('mappingInfoPanel').parentNode.parentNode.style.display = "none";

    var listMapping = $('mappingSelector');
        listMapping.selectedIndex = -1;
}

function existingMappingRadioClick(sender)
{
    showInfoPanel();

    var listMapping = $('mappingSelector');

    if (listMapping.selectedIndex == -1 && listMapping.options.length)
    {
        listMapping.selectedIndex = 0;

        //-------------------------------------------------
        // Dispatching 'onchange' event for IE
        //-------------------------------------------------
        if (listMapping.fireEvent)
        {
            var event = document.createEventObject();
            listMapping.fireEvent("onchange", event);
        }

        //-------------------------------------------------
        // Dispatching 'onchange' event for other browsers
        //-------------------------------------------------
        else
        {
            var event = document.createEvent("HTMLEvents");
                event.initEvent("change", true, false);

            listMapping.dispatchEvent(event);
        }

        mappingSelectorFocus();
    }

    return true;
}

function createStringCutter(charCounter)
{
    return function(strHolder, destWidth)
    {
        var str = strHolder.data;

        charCounter.firstChild.nodeValue = strHolder.data;

        if (charCounter.offsetWidth <= destWidth) {
            return str;
        }

        var arr = str.split("");
        var count = 0;

        charCounter.firstChild.nodeValue = "...";

        for (;count < arr.length; count++)
        {
            charCounter.firstChild.nodeValue = charCounter.firstChild.nodeValue + arr[count];

            if (charCounter.offsetWidth >= destWidth)
                break;
        }

        return str.substr(0, count) + "...";
    }
}


document.observe("dom:loaded", function()
{
    var cutStr = createStringCutter($$('.charCounter')[0]);
    var resizeTimer = null;

    //--------------------------------------------------------------------------------------
    //
    //--------------------------------------------------------------------------------------
    var clearInfoPanel = function()
    {
        var mappingInfoPanel = document.getElementById("mappingInfoPanel");
        /*
        var cells = mappingInfoPanel.getElementsByTagName("td");

        for (var i in cells)
        {
            cells[i].onmouseover = function(event)
            {
                alert(this);
            }
        }
        */

        for (var ri=0; ri<mappingInfoPanel.rows.length; ri++)
        {
            var row = mappingInfoPanel.rows[ri];

            for (var ci=0; ci<row.cells.length; ci++)
            {
                if (row.cells[ci].firstChild && row.cells[ci].firstChild.nodeName != "SPAN")
                {
                    if (row.cells[ci].data == undefined) {
                        row.cells[ci].data = row.cells[ci].firstChild.nodeValue;
                    }
                    row.cells[ci].removeChild(row.cells[ci].firstChild);
                }
            }
        }
    }

    //--------------------------------------------------------------------------------------
    //
    //--------------------------------------------------------------------------------------
    var renderInfoPanel = function()
    {
        var mappingInfoPanel = document.getElementById('mappingInfoPanel');
        var cells = mappingInfoPanel.getElementsByTagName("td");

        var tooltip = $$(".tooltipForMappedField")[0];
        var timer   = null;

        for (var i=0; i<cells.length; i++)
        {
            if (!cells[i].onmouseover)
            {
                cells[i].onmouseover = function(event)
                {
                    var cell = this;

                    try
                    {
                        if (cell.data != cell.firstChild.nodeValue)
                        {
                            cell.style.cursor = "pointer";
                            $(tooltip).update(cell.data);
                            tooltip.style.display = "block";
                            tooltip.style.visibility = "hidden";

                            timer = setTimeout(function() {
                                tooltip.style.visibility = "visible";
                            },
                            500);
                        }
                    }
                    catch(e){}
                }

                cells[i].onmousemove = function(event)
                {
                    var e = event||window.event;

                    var clientWidth = window.innerWidth||document.body.clientWidth;
                    var xPos = e.clientX;
                    var yPos = e.clientY;

                    if (xPos + tooltip.offsetWidth > clientWidth) {
                        xPos = clientWidth - tooltip.offsetWidth;
                    }

                    tooltip.style.left = xPos + "px";
                    tooltip.style.top  = yPos - 28 + "px";
                }

                cells[i].onmouseout = function(event)
                {
                    if (timer) {
                        clearTimeout(timer);
                    }

                    tooltip.style.display = "none";
                }
            }
        }

        for (var ri=1; ri<mappingInfoPanel.rows.length; ri++)
        {
            var row = mappingInfoPanel.rows[ri];

            var spField = cutStr(row.cells[0], row.cells[0].offsetWidth - 2); // 2 is paddings in cell
            var scField = cutStr(row.cells[1], row.cells[1].offsetWidth - 2);

            row.cells[0].appendChild(document.createTextNode(spField));
            row.cells[1].appendChild(document.createTextNode(scField));
        }
    }

    //--------------------------------------------------------------------------------------
    //
    //--------------------------------------------------------------------------------------
    window.updateInfoPanel = function()
    {
        if (resizeTimer) {
            clearTimeout(resizeTimer);
        }

        clearInfoPanel();
        resizeTimer = setTimeout(renderInfoPanel, 20);
    }

    window.onresize = updateInfoPanel;
});