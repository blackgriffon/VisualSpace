   M S B A M L   `   `   `  ����G  CVisual_Space, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:Nollan.Visual_SpaceNollan.Visual_Space  5   	   $    Nollan.Visual_Space.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   3local!clr-namespace:Nollan.Visual_Space     5   	     � Title$  
MainWindow��5   	   $��367.677��6   $��540.909��6-    � 	Resources 5	      #   Nollan.Visual_Space.MapCanvas   Key_mapCanvas��%   &	          5
   
   6   5      .��5      � -   5        $��mainGrid��5       � RowDefinitions 5   
   �� 5      �� 5      .+�5   
     -   5          Name$ 	mapCanvas��5   '   #   Nollan.Visual_Space.MapCanvas   MouseLeftButtonDown$# MapCanvas_MouseLeftButtonDown��5         	MouseMove$ MapCanvas_MouseMove��5         SizeChanged$ mapCanvas_SizeChanged��5         KeyDown$ mapCanvas_KeyDown��5         MouseLeftButtonUp$! MapCanvas_MouseLeftButtonUp��5      $��0��5      
������ .+�5      �� -   5        $��txtBox��5      $��50.534��6$   	 �� Left$	 408.206��63   

 �� Top$
 24.419��6I   �� 5      -   5        $��	지우기��5      $	 472.401��68   $
 24.419��6N   �� 5       -   5        $��Ch_line��5       $��선 그리기��6&   $	 418.849��66   $
 62.014��6L   5!   -   �� 5#      -   5        $��Ch_linePlusMinus��5#      $��선 증가감소��6/   $	 418.849��6@   $
 102.017��6V   $��Center=�6l   $��Center=�6�   $��29.55��5$   ]   $��93.552��6l   5&      5'      5(      