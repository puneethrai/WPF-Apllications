   M S B A M L   `   `   `  ����   twentyminute    twentyminute.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     � Title$  Twenty Twenty�� � AllowsTransparency .  � WindowStyle$
 None=� J� Opacity$ 1Q� � SizeToContent$ WidthAndHeight=� � 	Resources    TitleTextStyle �� 
TargetType �� Property �� Value	 �� EndPoint
 �� 
StartPoint � Color � Offset  CloseButton �� 
TargetType �� CornerRadius ��Name �� RadiusY �� Left �� Bottom	 �� X1	 �� X2	 �� Y1	 �� Y2 ��Name �� Triggers P� Property P� Value �� 
TargetName  MinimizeButton��%�  &	        &	 	    &	     �� " ��.���  � ��$	 Top=���  � ��$ Stretch=���  � ��$ Calibri)���  � ��$ 17(���  � ��$ 20����  � ��$
 Bold"���  � �� �� $	 0.5,1&�$
 0.5,0&�.��� $ 	#FF000000��$ 0Q�� $ 	#FF4B2C1B��$ 1Q��� " ��.R��� $��1,1,2,2q�$ 0,0,4,0�����#C98172���#C98172._�� .+��� $ MyRectangle��$��34��$��18��$ 20������ $	
 0,0&�$		 0,1&�.��� $ 0Q�$ #F89C8C��� $
 0.45Q�$ #D47F75��� $
 0.45Q�$ #C04C3C��� $ 1Q�$ #C98172���� $��34��$��18��.+��� $ 11��$ 5��
�������$ 0��$ 10��$ 10��$ 0��$��1���� $ 11��$ 5��
�������$ 0��$ 10��$ 00��$ 10��$��1���� $ Contents��$��Center=�$��Center=�$
��Auto��$��3,3,3,3q� P�  �@J�IsMouseOver . . ��� $ MyRectangle�� � �� �� $	
 0,0&�$		 0,1&�.��� $ 0Q�$ #F89C8C��� $
 0.45Q�$ #E36A53��� $
 0.45Q�$ #C72B0E��� $
 0.75Q�$ #D44310��� $ 1Q�$ #F5E478���� " ��.R��� $��2,1,1,2q�$ 0,0,0,4�����	#FF7E7EFC���	#FF6D6DF2._�� .+��� $ MyRectangle��$��31��$��18������ $	
 0,0&�$		 0,1&�.��� $ 0Q�$ 	#FFADBAFF��� $
 0.45Q�$ 	#FF8BA1D4��� $
 0.45Q�$ 	#FF1962C1��� $ 1Q�$ 	#FF99ACC4���� $��34��$��18��.+��� $ 11��$ 6��
�������$ 0��$ 10��$ 0��$ 0��$��1���� $ Contents��$��Center=�$��Center=�$
��Auto��$��3,3,3,3q� P�  �@J�IsMouseOver . . ��� $ MyRectangle�� � �� �� $	
 0,0&�$		 0,1&�.��� $ 0Q�$ 	#FF7373F8��� $
 0.45Q�$ 	#FF3C6DDE��� $
 0.45Q�$ 	#FF0C20BE��� $
 0.75Q�$ 	#FF5D27D8��� $ 1Q�$ 	#FFBCA3F5�� J� InputBindings ��  �� Command$
 Help��
 �� Key$ F1�� J� CommandBindings )  #System.Windows.Input.CommandBinding  -       Command$  ApplicationCommands.Help��.��� $	��307�����	#00000000$	��506��! � RowDefinitions! �� $	��18* ��� $	��283 ��� $��6* �.+��� -   $��	TimeLabel��$��Seconds Left:0��$��36��$
��Left=�$��
12,135,0,0q�$	��Top=�$	��182��$��Comic Sans MS)�$
��Bold"�$��Normal&�
���  ��$��20(�$��1���� -   $��MinuteLabel��$��Minutes Left:20��$��40��$
��Left=�$��270,135,0,0q�$	��Top=�$	��221��$��Comic Sans MS)�$
��Bold"�$��Normal&�
��� � �$��20(�$��1���� -   $��StopBtn��$
��STOP��$��39��$
��Left=�$��	8,244,0,0q�$	��Top=�$	��231��$��1���� -   $��StartBtn��$��START��$��39��$
��Left=�$��263,244,0,0q�$	��Top=�$	��231��$��1���� -   $��DisplayLabel��$��Work For Another 20 min��$��44��$
��Left=�$��8,19,0,0q�$	��Top=�$	��486��$��Arial)�$��24(�$
��Bold"�$��1���� -   $��
TitleBarSP��" �� Orientation$" 
Horizontal=�$
��Left=�$	��Top=�$��18��.+�� .+� � -	   $��
TitleFrame��$��18��$
��Left=�$	��Top=�$	��491�����	#00000000$ 1Q����� $	 1,0.5&�$
 0,0.5&�.��� $
 Blue��$ 0Q�� $ White��$ 1Q��� -
   $��AppName��$
��Left=�$��18��$��Twenty Twenty��$	��417��|�.  
������ #��[  
�������# K� ZIndex$# 10��$��6,-1,0,0q��� -   $��MinimizeBtn��
�������$��18��$
��Left=�$��	423,0,0,0q�#���  $	��Top=�$��34��$# 10���� -   $��CloseBtn��$��18��$��34��
�������$��
457,0,12,0q�#���   � -   $��
OuterFrame��$��3��$	��306��$
��Left=�$	��Top=�$	��506��$# -1�����	#FF4A39F8$��2q�