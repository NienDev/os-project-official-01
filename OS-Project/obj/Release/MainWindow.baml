   M S B A M L   `   `   `  ���� E  AOS-Project, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:OS_Project
OS_Project  =#clr-namespace:OS_Project.ViewModelsOS_Project.ViewModels  3clr-namespace:OS_Project.ViewsOS_Project.Views      OS_Project.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     *localclr-namespace:OS_Project     Z VFontAwesome.WPF, Version=4.7.0.37774, Culture=neutral, PublicKeyToken=0758b07a11a4f466-fa$http://schemas.fontawesome.io/icons/  4
viewmodels#clr-namespace:OS_Project.ViewModels  *viewsclr-namespace:OS_Project.Views  -     � Title$  
MainWindow��$	��800��$
��1200�� � WindowStartupLocation$ CenterScreen=� � WindowStyle$
 None=�
������  � AllowsTransparency .  � 	Resources  �� DataType-   'OS_Project.ViewModels.TreeViewViewModel   OS_Project.Views.TreeView � DataContext+   %OS_Project.ViewModels.MemberViewModel   OS_Project.Views.Members*   $OS_Project.ViewModels.AboutViewModel   OS_Project.Views.About   FocusVisual �� Property �� Value	 �� StrokeDashArray
 J� SnapsToDevicePixels  Button.Static.Background �� Color  Button.Static.Border  Button.MouseOver.Background  Button.MouseOver.Border  Button.Pressed.Background  Button.Pressed.Border  Button.Disabled.Background  Button.Disabled.Border 	 Button.Disabled.Foreground �� 
TargetType �� 
TargetType ��Name ��Name �� Triggers P� Property P� Value �� 
TargetName 
 FocusVisual1 �� BasedOn  
SideBarBtn �� CornerRadius 
 maxbtn �� Value �� Binding!  FontAwesome.WPF.FontAwesome   Icon��%W  (��       *, +)(�� (     *, +)(�� N     *, +)&	  s     &	 �     &	 �     &	 �     &	     &	     &	 0    &	 G    &	 \    &		 q    '�� �    7   7  7  7  7  7  7  7  7  7 	 &	
 ]    &	 �    7	�&	 �    �� TreeView��"  .R�   �� �� 
Member��"  .R�   �� �� 	About��"  .R�   �� �� .���  � �� �� .R��� $��2q�$		 1 2R�#��� ��
 . $��1���� $ 	#FFDDDDDD���� $ 	#FF707070���� $ Transparent���� $ Transparent���� $ Transparent���� $ Transparent���� $ 	#FFF4F4F4���� $ 	#FFADB2B5���� $ 	#FF838383���� " ��.���  �@�FocusVisualStyle8   ��  � ��8  ��  � ��8  ��  � ��# � ����  � ��$ 1q���  � ��$ Center=���  � ��$ Center=���  � ��$ 1q���  � �� �� " ��.R��� $ border��#��z��#��z��#��z��
 . ._��� $ contentPresenter��|�.  #��z��#��z����. #
 z
 #��z�� P�  �@��IsDefaulted . . ��� $ border��# � �� � ��P�  �@J�IsMouseOver . . ��� $ border��8   � ���� $ border��8   � ��P�  � �� . . ��� $ border��8   � ���� $ border��8   � ��P�  � {� .  . ��� $ border��8   � ���� $ border��8   � ���� $ contentPresenter��8 	  � ���� .���  � �� �� .R��� $��2q�$		 1 2R�#��� ��
 . $��1���� 8   " 	����  � �� �� " 	�.R��� $ SideIconBorder��$��80��$��80��$ 8��._��� $ SideBarIconPresenter��$��Center=� P�  �@J�IsMouseOver . . ��� $ SideIconBorder�� � �� �#D7E5FAP�  �@d�	IsChecked$
 TrueY�. ��� $ SideIconBorder�� � �� �#D7E5FA�� " ��.���  � �� �� " ��.R��� $ contentPresenter��$��Center=�$��Center=� �� $
 True�� �� *IsMaximized+.a��� $ contentPresenter�� � ��   
�������$��Right=�$ WindowRestore=�$��24(��� $ False�� �� *IsMaximized+.a��� $ contentPresenter�� � ��   
�������$��Right=�$ WindowMaximize=�$��24(�.���� $ 8 8 0 0 ��$	��Top=����#091F4B._��� $	��800������ *ActualWidth+ �� RelativeSource ��  �� AncestorType" �.+�� $��86�� � ColumnDefinitions �� �� .+�]� $��0��$��Center=�$��20 0 0 0q�.+��� $��Center=�$��20(�
�������$
��Bold"�$��Poppins)�NNP's Product�� $��center=�$��20 0 0 0q�$��16(�
�������$��Light"�$��Poppins)�
USB Reader�� $��Right=� �� Orientation$ 
Horizontal=�$��Center=�$��1��.+��� $��Center=�$��Center=�$��40��$��40��
������ 
������ ���� *MinimizeWindowCommand+.��  
�������$��Right=�$ WindowMinimize=�$��24(��� #���  $��Center=�$��Center=�$��40��$��40��
������ 
������ ���� *MaximizeWindowCommand+�� $��0 0 10 0q�$��Center=�$��Center=�$��40��$��40��
������ 
������ ���� *CloseWindowCommand+.��  $��4q�
�������$��Right=�$ Close=�$��24(��� $ 0 0 8 8�����#fff$
��Left=�._�� $	��714��$
��Left=����� *ActualWidth+ �� " � �� $��96 ��� $��* �.+��� $ 0 0 0 12��$��0��$��2q����#D7E5FA���#F8FAFE._��� -    ��Name$ SideBarContainer��.+�	� -    	�Name$ 
DefaultBtn��#��[ $��80��$��80��$��0 24 0 0q�.��  $
 Home=�$��40(����#061A49	� -   #��[ $��80��$��80��$��	0 24 0 24q�.��  $ 
InfoCircle=�$��40(����#061A49	� -   #��[ $��80��$��80��.��  $ Group=�$��40(����#061A49�� -    ��Name$ CC��$��1��