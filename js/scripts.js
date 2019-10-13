$(function () {
	$('.navbar-toggle' ).click(function(e) {
		 e.stopPropagation();
	  $('.navbar').toggle();
	  $('.navbar-collapse.collapse').toggle()
	});	

	
	var viewWidth = $(window).width();
	$('.nav').on('mouseleave', function(){
		if (viewWidth > 992) {
			$('.sub-menu').removeClass('open');
		}
	});

	$('.nav li').on('mouseenter', function(){
		if ($(this).hasClass('sub-menu')) {
			$(this).addClass('open');			
		}
	});

	$('.nav li').on('mouseleave', function(){
		if ($(this).hasClass('sub-menu')) {
			$(this).removeClass('open');
		}
	})

	$('a.parentItem').click(function(){
		var target = $(this).attr('data-target');
		if($(target).is(":visible") ) {
		  window.location.href = $(this).attr('attr-url');
		}

	});
	

});