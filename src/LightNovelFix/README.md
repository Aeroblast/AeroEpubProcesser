# LightNovel Fix
主要针对一些翻译/录入的轻小说制作成的EPUB进行处理。
## Line Height Fix
针对行高和段间距。小说这种碎言碎语的东西，我很不喜欢加上段间距……况且国内出版的小说也基本都是大行高+无段间距,日文原文也是没有段间距……又不是新闻那种大段大段的，连续对话看着真难受……

处理上，因为制作者的样式往往直接使用Tagname选择，因此也直接在CSS后面加一个同样优先级的样式覆盖。

## Text Indent Fix
轻小说的标点符号是不符合中文的规范的，那些自动的排版、字体也自然不会对 ```「『``` 这种引号做优化，直接用会让原本2em的首行缩进看起来更大。

自从某次试着把所有```「『（``` 都往前挪了0.5em就再也回不来了……0.5em可以基本让这种括号和一般的字对齐。如果行高比较小，再多往外一点会有一种微妙的平衡感……不过这里没有做，因为按照行高修正的值减0.5em就好了。

## Meta Fix
删了dc:creator标签中opf:file-as属性的值。这个玩意已经有新的写法了（meta里什么refine啥的），本来也没人用。本意是写一个排序友好的名称，不知道为什么都流行写epub制作者的代号……那个感觉写contributor什么的里面更好吗。反正也有在其他位置写，直接干掉了。

## Separator Centralize
把分割符号居中。
事实上，日文原书确实是不居中的，而且看起来还行。可是人家是竖排的，看起来是比一般文字多缩进了一点；搞成横排尤其放到手机上就是中间偏一点，逼死强迫症……

预设几个常用分割符。极短段落会报告可疑。
