function showHidePopUp(item)
{
    var style = item.nextSibling.style;
        style.display = (style.display == 'none') ? 'block':'none';
}

function openSharepointDocument(fileUrl)
{
    fullFileUrl = fileUrl;
    editObject = null;

    //---------------------------------------------------------
    // Creating new ActiveX object
    //---------------------------------------------------------
    try
    {
        editObject = new ActiveXObject("Sharepoint.OpenDocuments.2");

        if (editObject != null)
        {
            if (typeof (Sitecore) != "undefined" && Sitecore.WebEdit != null) {
                editObject.EditDocument(fullFileUrl);
            }
            else {
                editObject.ViewDocument(fullFileUrl);
            }
        }
    }

    //---------------------------------------------------------
    // Client doesn't support ActiveX or it isn't installed
    //---------------------------------------------------------
    catch(e)
    {
        window.location = fullFileUrl;
    }
}

function createStringCutter(charCounter)
{
    return function(strHolder, destWidth)
    {
        try
        {
            var str = strHolder.firstChild.nodeValue;

            if (strHolder.offsetWidth <= destWidth) {
                return str;
            }

            var arr = str.split("");
            var count = 0;

            charCounter.text("...");

            for (;count < arr.length; count++)
            {
                charCounter.text(charCounter.text() + arr[count]);

                if (charCounter.width() >= destWidth)
                    break;
            }

            return str.substr(0, count) + "...";
        }
        catch(e)
        {
            return "";
        }
    }
}

function getAncestor(element, ancestorClassName)
{
    for (var ancestor = element.parentNode; ancestor.parentNode; ancestor = ancestor.parentNode)
    {
        if (ancestor.className.match(ancestorClassName)) {
            return ancestor;
        }
    }

    return null;
}

function initTooltip(table, cell)
{
    table.showTimeout = null;
    table.hideTimeout = null;

    if (cell.firstChild.firstChild.nodeValue.match(/[.]{3}$/g))
    {
        var showTooltip = function()
        {
            clearTimeout(table.showTimeout);
            clearTimeout(table.hideTimeout);

            table.showTimeout = setTimeout(function() {
                table.tooltip.animate({opacity: 'show'}, 100)
            },
            500);
        }

        var hideTooltip = function()
        {
            clearTimeout(table.showTimeout);

            table.hideTimeout = setTimeout(function() {
                table.tooltip.hide();
            },
            20);
        }

        jQuery(cell).mouseover(function(e)
        {
            this.offset   = jQuery(cell).offset();
            this.position = jQuery(cell).position();

            table.tooltip.text(cell.tooltipText);

            showTooltip();
        });

        jQuery(cell).mousemove(function(e)
        {
            this.offset   = jQuery(this).offset();
            this.position = jQuery(this).position();

            table.tooltip.css({left: e.pageX - this.offset.left + this.position.left, top: e.pageY - this.offset.top + this.position.top - 40});
        });

        if (window.addEventListener) {
            cell.addEventListener("mouseout", function(e) { hideTooltip(); }, false);
        }
        else
        {
            cell.attachEvent("onmouseout", function(e) {
                hideTooltip();
            });
        }
    }
}

function truncateTable(table)
{
    try
    {
        var charCounter = jQuery('.charCounter', table.parentNode);
        var cutStr      = createStringCutter(charCounter);
        var rows        = jQuery('tr', table);
        var cellCount   = rows[0].cells.length;

        for (var i = cellCount-1; i >= 0; i--)
        {
            var gridWidth = table.offsetWidth;
            var overWidth = gridWidth - 500;

            if (gridWidth <= 500) {
                break;
            }

            rows.each(function(index)
            {
                var cell = this.cells[i];

                var cellWidthLimit  = cell.className.match("docName") ? 200:80;
                var targetCellWidth = cell.offsetWidth - overWidth;

                if (targetCellWidth < cellWidthLimit)
                    targetCellWidth = cellWidthLimit;

                if (cell.firstChild)
                {
                    if (cell.firstChild.firstChild && cell.firstChild.firstChild.nodeType == 3)
                    {
                        cell.tooltipText = cell.firstChild.firstChild.nodeValue;
                        cell.firstChild.firstChild.nodeValue = cutStr(cell.firstChild, targetCellWidth)

                        initTooltip(table, cell);
                    }

                    //--------------------------------------------------------------------------
                    // Hike for field "Created By" of table "search-result" of SP Search Control
                    //--------------------------------------------------------------------------
                    /*
                    if (table.className.match(/search-result/))
                    {
                        var tagString = jQuery("string", cell);

                        if (tagString.length)
                        {
                            var str = tagString.text();
                            jQuery("arrayofstring", cell).remove();

                            jQuery(cell).html("<span>" + str + "</span>");

                            cell.tooltipText = cell.firstChild.firstChild.nodeValue;
                            cell.firstChild.firstChild.nodeValue = cutStr(cell.firstChild, targetCellWidth)

                            initTooltip(table, cell);
                        }
                    }
                    */

                    //--------------------------------------------------------------------
                    // For RichText in cell like 'Body' field in Announcement List
                    //--------------------------------------------------------------------
                    var node = cell.firstChild.firstChild;
                    var bodyWidth = 300;

                    if (node && node.className && node.className.match(/^ExternalClass/g))
                    {
                        node.parentNode.style.whiteSpace = "normal";

                        node.parentNode.style.width = bodyWidth + "px";
                        node           .style.width = bodyWidth + "px";
                        cell           .style.width = bodyWidth + "px";
                    }
                }
            });
        }
    }
    catch(e){/*alert(e.message)*/}
}

function lockControls(sender)
{
    var currentControl = sender; //getAncestor(sender, 'control-wrapper');

    jQuery('.control-wrapper').each(function(index, val)
    {
        if (currentControl == this)
        {
            var faderContainer = jQuery('.progressBackgroundFilter', this).parent().addClass('faderContainer');
                faderContainer.css({width: this.offsetWidth, height: this.offsetHeight, display: 'block'});
        }
        else
        {
            var disabler = jQuery('.disableScreen', this);
                disabler.css({width: this.offsetWidth, height: this.offsetHeight, display: 'none', display: 'block'});
        }
    });
}

function unlockControls()
{
    jQuery('.control-wrapper').each(function(index, val)
    {
        var disabler = jQuery('.disableScreen', this);
            disabler.css({width: 0, height: 0, display: 'none'});
    });
}

function initSearchControl(control)
{
    var link = jQuery('.advancedSearchLink', control);
    var advancedSearch = jQuery('.advancedSearch', control)

    if (advancedSearch.length)
    {
        link[0].href = "#";

        link[0].onclick = function()
        {
            advancedSearch.css("display") == "none"?
                advancedSearch.show():
                advancedSearch.hide();
        }
    }
}

function initHoverIntent(gridClassName)
{
    $j = jQuery.noConflict();

    jQuery('.action_popupspan > div').hover
    (
        function() {
            jQuery(this).addClass("hover");
        },

        function() {
            jQuery(this).removeClass("hover");
        }
    )

    //--------------------------------------------------------------------------------------------
    // Detecting if doctype is XHTML
    //--------------------------------------------------------------------------------------------
    var doctype = {
        node : null,
        xhtml: false
    };

    if (document.firstChild.nodeType == 8 && document.firstChild.nodeValue)
    {
        if (document.firstChild.nodeValue.match(/DOCTYPE/))
            doctype.node = document.firstChild.nodeValue;

        if (doctype.node)
            doctype.xhtml = doctype.node.match(/DTD XHTML/) != null;
    }

    //--------------------------------------------------------------------------------------------
    // Wolking through controls on page and set their z-index property in back order
    //--------------------------------------------------------------------------------------------
    var controls = jQuery('.control-wrapper');
    var length = controls.length

    unlockControls();

    controls.each(function(index, val)
    {
        if (this.className.match(/sharepoint-search/)) {
            initSearchControl(this);
        };

        var currentControl = this;

        this.style.zIndex = length - index;

        //--------------------------------------------------------------------
        // Tracking all cases when fader and disabler could appear
        //--------------------------------------------------------------------
        jQuery(".genericGrid_controlButtons a", this).click(function(event)
        {
            if (!this.className.match("disable")) {
                event.stopPropagation();  
                lockControls(currentControl);
            }
        });

        jQuery(".spSearchBtn", this).bind("click", function(event){
            event.stopPropagation();
            lockControls(currentControl);
        });

        jQuery("th a", this).bind("click", function(event){
            event.stopPropagation();
            lockControls(currentControl);            
        });

        jQuery(".advancedSearchLink", this).bind("click", function(event)
        {
            if (jQuery('.advancedSearch').length == 0) {
                event.stopPropagation();
                lockControls(currentControl);
            }
        });

        jQuery(".viewsList", this).bind("change", function(event){
            event.stopPropagation();
            lockControls(currentControl);
        })
    });

    //-------------------------------------------------------------------------------------------
    // The 'grid' variable will be undefined if this method was called withing SP Search control.
    //-------------------------------------------------------------------------------------------
    try {
        var grid = jQuery('.' + gridClassName + ', .search-result');
    }

    catch(e) {
        return;
    }

    grid.each(function(index, val)
    {
        jQuery(this).after("<span class='tooltip'></span>");
        jQuery(this).after("<div class='charCounter'>Sample string</div>")
        jQuery(this).after("<div class='action'><div class='btnAction'></div><div class='listAction'></div></div>");

        this.tooltip = jQuery('.tooltip', this.parentNode);
        this.action  = jQuery('.action ', this.parentNode);

        //---------------------------------------------------------------------
        // Defining cell which contains document link
        //---------------------------------------------------------------------
        var cellDocName = jQuery('.NameCell', grid);
            cellDocName.addClass('docName');

        if (this.className.match(/^search-result/))
        {
            if (this.rows.length > 1) {
                jQuery(".header", this).show();
            }
        }

        truncateTable(this);
    });

    var config =
    {
        over: function(e)
        {
            debug("hover ", this);

            var row = this;
            var widestCell = row.cells[0]||null;

            //-------------------------------------------------------------------------
            // Detecting the widest cell in row
            //-------------------------------------------------------------------------
            for (var i=0; i<row.cells.length; i++)
            {
                if (widestCell && widestCell.offsetWidth < row.cells[i].offsetWidth) {
                    widestCell = row.cells[i];
                }
            }

            var actionCell = jQuery('.NameCell', row);

            if (actionCell.length == 0) {
                actionCell = jQuery(widestCell);
            }

            actionCell.addClass('hover');

            if (actionCell[0]) // if actionCell present
            {
                var position = {
                    x: Math.round(actionCell.position().left),
                    y: Math.round(actionCell.position().top),
                    w: Math.round(actionCell[0].offsetWidth),
                    h: Math.round(actionCell[0].offsetHeight)
                };

                var action_popupspan = jQuery('.action_popupspan', row);

                //---------------------------------------------------------------------------------------
                // Action dropdown behaviour
                //---------------------------------------------------------------------------------------
                var action =
                {
                    obj    : row.parentNode.parentNode.action,
                    element: row.parentNode.parentNode.action[0],
                    timeout: null,

                    show: function()
                    {
                        this.obj.css({left: position.x, top: position.y + position.h, display: "block"});
                        return this;
                    },

                    hide: function()
                    {
                        listAction.hide();
                        this.element.style.display = "none";
                    },

                    mouseover: function(fn) {
                        this.element.onmouseover = fn;
                    },

                    mouseout: function(fn) {
                        this.element.onmouseout = fn;
                    }
                }

                //---------------------------------------------------------------------------------------
                // btnAction behaviour
                //---------------------------------------------------------------------------------------
                var btnAction =
                {
                    obj    : jQuery('.btnAction', action.element),
                    element: jQuery('.btnAction', action.element)[0],

                    show: function()
                    {
                        if (this.element.attachEvent)
                        {
                            if (doctype.xhtml)
                                this.obj.css({left: position.w - 16, top: -position.h, height: position.h-2});
                            else
                                this.obj.css({left: position.w - 16, top: -position.h, height: position.h});
                        }
                        else {
                            this.obj.css({left: position.w - 18, top: -position.h, height: position.h - 2});
                        }
                        return this;
                    },

                    click: function(fn) {
                        this.element.onclick = fn;
                    }
                }
                .show();

                //---------------------------------------------------------------------------------------
                // listAction behaviour
                //---------------------------------------------------------------------------------------
                var listAction =
                {
                    obj    : jQuery('.listAction', action.element),
                    element: jQuery('.listAction', action.element)[0],
                    timeout: null,
                    visible: 0,

                    show: function()
                    {
                        this.element.innerHTML = action_popupspan.html();
                        this.element.style.display = "block";

                        var w = position.w - 2;

                        if (this.element.attachEvent) { // For IE
                            w = position.w;
                        }

                        if (this.element.offsetWidth < position.w - 2) {
                            this.obj.css({width: w, display: "block"});
                        }

                        this.visible = 1;
                        return this;
                    },

                    hide: function()
                    {
                        this.element.style.display = "none";
                        this.visible = 0;
                    },

                    mouseover: function(fn) {
                        this.element.onmouseover = fn;
                    },

                    mouseout: function(fn) {
                        this.element.onmouseout = fn;
                    }
                }

                //---------------------------------------------------------------------------------------
                // currentRow selection behaviour
                //---------------------------------------------------------------------------------------
                var currentRow =
                {
                    obj    : jQuery(row),
                    element: row,
                    timeout: null,

                    mouseover: function(fn) {
                        this.element.onmouseover = fn;
                    },

                    mouseout: function(fn) {
                        this.element.onmouseout = fn;
                    },

                    focus: function()
                    {
                        this.obj.addClass("hover");

                        if (this.element.className.match(/genericGrid/)) {
                            action.show();
                        }

                        return this;
                    },

                    blur: function()
                    {
                        action.hide();
                        this.obj.removeClass("hover");
                    }
                }

                //---------------------------------------------------------------------------------------
                // Action dropdown logic
                //---------------------------------------------------------------------------------------
                btnAction.click(function(event) {
                    listAction.visible ? listAction.hide():listAction.show();
                })

                action.mouseover(function(event)
                {
                    clearTimeout(currentRow.timeout);
                    clearTimeout(listAction.timeout);
                    clearTimeout(action.timeout);
                })

                action.mouseout(function(event)
                {
                    action.timeout = setTimeout(function()
                    {
                        action.hide();
                        currentRow.blur();
                    },
                    100);
                });

                listAction.mouseover(function(event)
                {
                  $j("a", this).filter(function() { return !$(this).attr('nolock'); }).click(function(event)
                    {
                        lockControls(getAncestor(listAction.element, 'control-wrapper'));
                        action.hide();
                    })
                })

                listAction.mouseout(function(event)
                {
                    listAction.timeout = setTimeout(function() {
                        listAction.hide();
                    },
                    100);
                })

                currentRow.mouseover(function(event)
                {
                    clearTimeout(action.timeout);
                    clearTimeout(currentRow.timeout);
                });

                currentRow.mouseout(function(event)
                {
                    currentRow.timeout = setTimeout(function() {
                        currentRow.blur();
                    },
                    100);
                });

                currentRow.focus();
            }
        },

        timeout: 0,
        out: function(e) {}
    }

    var className = "." + gridClassName;
    $j(document).ready(function() { $j(className + " tr").hoverIntent(config); });
}

function debug(message, object) {
    if (typeof (console) != "undefined") {
        console.log(object + ": " + message);
    }
}

function goToFolder(folderName, folderPathClientId)
{
   var folderContainerId = folderPathClientId;
   var folderPathField = document.getElementById(folderContainerId);
   var oldFolderPath = folderPathField.value;
   var newFolderPath = folderName + "/";

   if (oldFolderPath) {
      newFolderPath = oldFolderPath + folderName + "/";
   }
   setFolder(newFolderPath, folderPathClientId);
}

function setFolder(folderPath, folderPathClientId)
{
   var folderContainerId = folderPathClientId;
   var folderPathField = document.getElementById(folderContainerId);
   folderPathField.value = folderPath;

   lockControls(getAncestor(folderPathField, 'control-wrapper'));
}