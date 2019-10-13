// Quick feature detection
function isTouchEnabled() {
	return (('ontouchstart' in window)
		|| (navigator.MaxTouchPoints > 0)
		|| (navigator.msMaxTouchPoints > 0));
}

$(function(){
	addEvent('map_1');
	addEvent('map_2');
	addEvent('map_3');
	addEvent('map_4');
	addEvent('map_5');
	addEvent('map_6');
	addEvent('map_7');
	addEvent('map_8');
	addEvent('map_9');
	addEvent('map_10');
	addEvent('map_11');
	addEvent('map_12');
	addEvent('map_13');
	addEvent('map_14');
	addEvent('map_15');
	addEvent('map_16');
	addEvent('map_17');
	addEvent('map_18');
	addEvent('map_19');
	addEvent('map_20');
	addEvent('map_21');
	addEvent('map_22');
	addEvent('map_23');
	addEvent('map_24');
	addEvent('map_25');
	addEvent('map_26');
	addEvent('map_27');
	addEvent('map_28');
	addEvent('map_29');
	addEvent('map_30');
	addEvent('map_31');
	addEvent('map_32');
	addEvent('map_33');
	addEvent('map_34');
	addEvent('map_35');
	addEvent('map_36');
	addEvent('map_37');
	addEvent('map_38');
	addEvent('map_39');
	addEvent('map_40');
	addEvent('map_41');
	addEvent('map_42');
	addEvent('map_43');
	addEvent('map_44');
	addEvent('map_45');
	addEvent('map_46');
	addEvent('map_47');
	addEvent('map_48');
	addEvent('map_49');
	addEvent('map_50');
	addEvent('map_51');
	addEvent('map_52');
	addEvent('map_53');
	addEvent('map_54');
})
$(function(){
	if($('#lakes').find('path').eq(0).attr('fill') != 'undefined'){
		$('#lakes').find('path').attr({'fill':map_config['default']['lakesColor']}).css({'stroke':map_config['default']['borderColor']});
	}

	$('#map-tip').css({
		'box-shadow':'1px 2px 4px '+map_config['default']['hoverShadow'],
		'-moz-box-shadow':'2px 3px 6px '+map_config['default']['hoverShadow'],
		'-webkit-box-shadow':'2px 3px 6px '+map_config['default']['hoverShadow'],
	});

	if($('#shadow').find('path').eq(0).attr('fill') != 'undefined'){
		var shadowOpacity = map_config['default']['shadowOpacity'];
		var shadowOpacity = parseInt(shadowOpacity);
		if (shadowOpacity >=100){shadowOpacity = 1;}else if(shadowOpacity <=0){shadowOpacity =0;}else{shadowOpacity = shadowOpacity/100;}
		
		$('#shadow').find('path').attr({'fill':map_config['default']['mapShadow']}).css({'fill-opacity':shadowOpacity})
	}
});

function addEvent(id,relationId){
	var _obj = $('#'+id);
	var _Textobj = $('#'+id+','+'#'+map_config[id]['namesId']);

	$('#'+['text-abb']).attr({'fill':map_config['default']['namesColor']});

	_obj.attr({'fill':map_config[id]['upColor'],'stroke':map_config['default']['borderColor']});
	_Textobj.attr({'cursor':'default'});
	if(map_config[id]['enable'] == true){
		if (isTouchEnabled()) {
			//clicking effect
			_Textobj.on('touchstart', function(e){
				var touch = e.originalEvent.touches[0];
				var x=touch.pageX+10, y=touch.pageY+15;
				var tipw=$('#map-tip').outerWidth(), tiph=$('#map-tip').outerHeight(), 
				x=(x+tipw>$(document).scrollLeft()+$(window).width())? x-tipw-(20*2) : x
				y=(y+tiph>$(document).scrollTop()+$(window).height())? $(document).scrollTop()+$(window).height()-tiph-10 : y
				$('#'+id).css({'fill':map_config[id]['downColor']});
				$('#map-tip').show().html(map_config[id]['hover']);
				$('#map-tip').css({left:x, top:y})
			})
			_Textobj.on('touchend', function(){
				$('#'+id).css({'fill':map_config[id]['upColor']});
				if(map_config[id]['target'] == 'new_window'){
					window.open(map_config[id]['url']);	
				}else if(map_config[id]['target'] == 'same_window'){
					window.parent.location.href=map_config[id]['url'];
				}
			})
		}
		_Textobj.attr({'cursor':'pointer'});
		_Textobj.hover(function(){
			//moving in/out effect
			$('#map-tip').show().html(map_config[id]['hover']);
			_obj.css({'fill':map_config[id]['overColor']})
		},function(){
			$('#map-tip').hide();
			$('#'+id).css({'fill':map_config[id]['upColor']});
		})
		//clicking effect
		_Textobj.mousedown(function(){
			$('#'+id).css({'fill':map_config[id]['downColor']});
		})
		_Textobj.mouseup(function(){
			$('#'+id).css({'fill':map_config[id]['overColor']});
			if(map_config[id]['target'] == 'new_window'){
				window.open(map_config[id]['url']);	
			}else if(map_config[id]['target'] == 'same_window'){
				window.parent.location.href=map_config[id]['url'];
			}
		})
		_Textobj.mousemove(function(e){
			var x=e.pageX+10, y=e.pageY+15;
			var tipw=$('#map-tip').outerWidth(), tiph=$('#map-tip').outerHeight(), 
			x=(x+tipw>$(document).scrollLeft()+$(window).width())? x-tipw-(20*2) : x
			y=(y+tiph>$(document).scrollTop()+$(window).height())? $(document).scrollTop()+$(window).height()-tiph-10 : y
			$('#map-tip').css({left:x, top:y})
		})
	}	
}

//The pins code
$(function(){
	if($('#pin-shadow').find('path').eq(0).attr('fill') != 'undefined'){
		var pinShadowOpacity = pin_config['default']['pinShadowOpacity'];
		var pinShadowOpacity = parseInt(pinShadowOpacity);
		if (pinShadowOpacity >=100){pinShadowOpacity = 1;}else if(pinShadowOpacity <=0){pinShadowOpacity =0;}else{pinShadowOpacity = pinShadowOpacity/100;}

		$('#pin-shadow').find('path').attr({'fill':pin_config['default']['pinShadow']}).css({'fill-opacity':pinShadowOpacity})
};


	var points_len = pin_config['points'].length;
	if( points_len > 0){
		var xmlns = "http://www.w3.org/2000/svg";
		var tsvg_obj = document.getElementById("map_points");
		var svg_circle,svg_rect;
		for(var i=0;i<points_len;i++){
			if (pin_config['points'][i]['shape']=="circle"){
				svg_circle = document.createElementNS(xmlns, "circle");
				svg_circle.setAttributeNS(null, "cx", pin_config['points'][i]['pos_X']+1);
				svg_circle.setAttributeNS(null, "cy", pin_config['points'][i]['pos_Y']+1);
				svg_circle.setAttributeNS(null, "r", pin_config['points'][i]['diameter']/2);
				svg_circle.setAttributeNS(null, "fill", pin_config['default']['pinShadow']);
				svg_circle.setAttributeNS(null, "style",'fill-opacity:'+pinShadowOpacity);
				svg_circle.setAttributeNS(null, "id",'map_points_shadow_'+i);
				tsvg_obj.appendChild(svg_circle);
				svg_circle = document.createElementNS(xmlns, "circle");
				svg_circle.setAttributeNS(null, "cx", pin_config['points'][i]['pos_X']);
				svg_circle.setAttributeNS(null, "cy", pin_config['points'][i]['pos_Y']);
				svg_circle.setAttributeNS(null, "r", pin_config['points'][i]['diameter']/2);
				svg_circle.setAttributeNS(null, "fill", pin_config['points'][i]['upColor']);
				svg_circle.setAttributeNS(null, "stroke",pin_config['points'][i]['outline']);
				svg_circle.setAttributeNS(null, "stroke-width",pin_config['points'][i]['thickness']);
				svg_circle.setAttributeNS(null, "id",'map_points_'+i);
				tsvg_obj.appendChild(svg_circle);
				dynamicAddEvent(i);
			}
			else if(pin_config['points'][i]['shape']=="rectangle"){
				svg_rect = document.createElementNS(xmlns, "rect");
				svg_rect.setAttributeNS(null, "x", pin_config['points'][i]['pos_X']- pin_config['points'][i]['width']/2+1);
				svg_rect.setAttributeNS(null, "y", pin_config['points'][i]['pos_Y']- pin_config['points'][i]['height']/2+1);
				svg_rect.setAttributeNS(null, "width", pin_config['points'][i]['width']);
				svg_rect.setAttributeNS(null, "height", pin_config['points'][i]['height']);
				svg_rect.setAttributeNS(null, "fill", pin_config['default']['pinShadow']);
				svg_rect.setAttributeNS(null, "style",'fill-opacity:'+pinShadowOpacity);
				svg_rect.setAttributeNS(null, "id",'map_points_shadow_'+i);
				tsvg_obj.appendChild(svg_rect);
				svg_rect = document.createElementNS(xmlns, "rect");
				svg_rect.setAttributeNS(null, "x", pin_config['points'][i]['pos_X']- pin_config['points'][i]['width']/2);
				svg_rect.setAttributeNS(null, "y", pin_config['points'][i]['pos_Y']- pin_config['points'][i]['height']/2);
				svg_rect.setAttributeNS(null, "width", pin_config['points'][i]['width']);
				svg_rect.setAttributeNS(null, "height", pin_config['points'][i]['height']);
				svg_rect.setAttributeNS(null, "fill", pin_config['points'][i]['upColor']);
				svg_rect.setAttributeNS(null, "stroke",pin_config['points'][i]['outline']);
				svg_rect.setAttributeNS(null, "stroke-width",pin_config['points'][i]['thickness']);
				svg_rect.setAttributeNS(null, "id",'map_points_'+i);
				tsvg_obj.appendChild(svg_rect);
				dynamicAddEvent(i);
			}
		}
	}
});

function dynamicAddEvent(id){
	var obj = $('#map_points_'+id);

	if(pin_config['points'][id]['enable'] == true){
		if (isTouchEnabled()) {
			obj.on('touchstart', function(e){
				var touch = e.originalEvent.touches[0];
				var x=touch.pageX+10, y=touch.pageY+15;
				var tipw=$('#map-tip').outerWidth(), tiph=$('#map-tip').outerHeight(),
				x=(x+tipw>$(document).scrollLeft()+$(window).width())? x-tipw-(20*2) : x
				y=(y+tiph>$(document).scrollTop()+$(window).height())? $(document).scrollTop()+$(window).height()-tiph-10 : y
				$('#'+id).css({'fill':pin_config['points'][id]['downColor']});
				$('#map-tip').show().html(pin_config['points'][id]['hover']);
				$('#map-tip').css({left:x, top:y})
			})
			obj.on('touchend', function(){
				$('#'+id).css({'fill':pin_config['points'][id]['upColor']});
				if(pin_config['points'][id]['target'] == 'new_window'){
					window.open(pin_config['points'][id]['url']);
				}else if(pin_config['points'][id]['target'] == 'same_window'){
					window.parent.location.href=pin_config['points'][id]['url'];
				}
			})
		}
		obj.attr({'cursor':'pointer'});
		obj.hover(function(){
			$('#map-tip').show().html(pin_config['points'][id]['hover']);
			obj.css({'fill':pin_config['points'][id]['overColor']})
		},function(){
			$('#map-tip').hide();
			obj.css({'fill':pin_config['points'][id]['upColor']});
		})
		//clicking effect
		obj.mousedown(function(){
			obj.css({'fill':pin_config['points'][id]['downColor']});
		})
		obj.mouseup(function(){
			obj.css({'fill':pin_config['points'][id]['overColor']});
			if(pin_config['points'][id]['target'] == 'new_window'){
				window.open(pin_config['points'][id]['url']);	
			}else if(pin_config['points'][id]['target'] == 'same_window'){
				window.parent.location.href=pin_config['points'][id]['url'];
			}
		})
		obj.mousemove(function(e){
				var x=e.pageX+10, y=e.pageY+15;
				var tipw=$('#map-tip').outerWidth(), tiph=$('#map-tip').outerHeight(), 
				x=(x+tipw>$(document).scrollLeft()+$(window).width())? x-tipw-(20*2) : x
				y=(y+tiph>$(document).scrollTop()+$(window).height())? $(document).scrollTop()+$(window).height()-tiph-10 : y
				$('#map-tip').css({left:x, top:y})
		})
	}	
}
