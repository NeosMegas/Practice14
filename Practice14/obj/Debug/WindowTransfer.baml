   M S B A M L   `   `   `  ����E  APractice14, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Practice14
Practice14  5   	       Practice14.WindowTransfer   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:Practice14     5   	   -   5          � Title$#  Перевод средств��5   	   $	��120��6!   $	��400��6.    � WindowStartupLocation$ CenterScreen=�5
   	    � SizeToContent$ Height=�6.   .��5      � .+�5   
   ��  � 	Resources 5       �� 
TargetType �� Property �� Value��%�   '��       '�� G     �� 5      " ��6   .�5      ��  � ��$ 5q�6/   5      �� 5      " ��6   .�5      ��  � ��$ 5q�6/   5      5      .+�5      �� -   5         ��Name$ tbInfo��5      $
��Wrap=�5      $
��info��5      �� 5      -   5         ��Name$ tbAmount��5      $��5q�5      $��0��5      .��5      ��5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   1   �� 6   6sys&clr-namespace:System;assembly=mscorlib     61   " ��6   	 �� 	Resources	 5         CueBannerBrush
 j� 
AlignmentX j� 
AlignmentY j� Stretch !� Visual��%�   &	        !� 5       $

 Left=�6A   $ Center=�6S   $
 None=�6g    5!   "   �� 5"   &   $��
Сумма��6,   
�������6<   5$      5%       �� Triggers 5&      P� 5'       P� Property � �� �� Empty P� Value# Z 66   . �5(   "   ��  � ��# [  6?   5)      P� 5*       � �� 66   Z� . �5+   "   ��  � ��# [  6?   5,      P� 5-       �@J�IsKeyboardFocused . . �5.   "   ��  � ��
 �����6?   5/      51      .��52   !   53      �� 54      -   5         ��Name$ btnOK��54      $��OK��55      6"   �� 56      -   5        $ 	btnCancel��56      $��Cancel��57      6&   58      5:      5;      