# Axaprj.LexFeatExtr
Text Feature Extraction Formal Grammar Engine 
- Takenet.Textc involved, uses own [Takenet.Textc .Net Core port](https://github.com/Axaprj/textc-csharp/tree/port2core)
- common info [Formal Grammar](https://en.wikipedia.org/wiki/Formal_grammar)
- common info [Backus–Naur form](https://en.wikipedia.org/wiki/Backus%E2%80%93Naur_form)

## code
VS 2017, VS 2019, .NET Core

## terms and naming
- `Terminal Symbol` - A Symbol that presented a particular input token or token sequence. The base element of `Expressions`. 
- `Non-Terminal Symbol` - A Symbol that presented a particular `Rule`. The base element of `Expressions` and `Rules`.  
- `Rule` - Matching non-terminal symbol to `Expressions`(joined by OR, ordered).
- `Expression` - Boolean expression 
	(joined by AND `Symbols`, a Terminal Symbol is true when it conforms to corresponding by order input token, 
	`Non-Terminal Symbol` is true when its Rule is true). 
- `Grammar` - Set of `Rules` with a start Symbol.
- `Product` - Objects created when an input token is matching to `Symbol`. Non-terminal Products are collections of Products.
- `Product` - The whole text-processing result. Is a root node of tree with non-terminal Product nodes and terminal Product leaves.
- `Production` - Production `Produces` `Product`.
- `Symbol Attribute` - Specification of a `Symbol`. Implements token matching and Product creation.

## example
Sample of specialized texts parsing. Randomly selected parts of specialized texts are used.
Easy spacing preprocessing required.
See the grammar in source [Axaprj.LexFeatExtr.Tests.ConsumptionDemo](https://github.com/Axaprj/LexFeatExtr/blob/master/Tests/Axaprj.LexFeatExtr.Tests/ConsumptionDemo.cs).
Texts and processing results are following:

- "its combined fuel consumption is reduced to up to 2.4 liters per 100 kilometers ( 98 mpg US ) , with CO 2 emissions of up to 56 grams per kilometer ."
```
Consumption[combined consumption 
	Vals[Val[val:2.4 l100km] ( Val[val:98 mpg] )]
]
CO2Val[val:56 gkm]
```

- "The lowest combined fuel consumption for the range is 7.9 L / 100 km for LS-U and LS-T 4 x 4 autos ( 209 CO 2 emissions ) , and peaks at 8.1 L / 100 km ( for all LS-T and LS-U 4 x 2 and LS-M 4 x 4 ) ."
```
Consumption[combined consumption 
    Vals[Val[val:7.9 l100km]]
]
Val[val:8.1 l100km]
```

- "Its combined fuel consumption is 141.2 - 122.8 mpg , while CO 2 emissions are at least 49 g / km ."
```
Consumption[combined consumption 
	Vals[Val[[vfrom: 141.2, vto: 122.8] mpg]]
] 
CO2Val[val:49 gkm]
```

- "NEDC combined fuel consumption is 4.2 - 4.1 l / 100 km ( preliminary fuel consumption NEDC 1 : urban 4.9 - 4.8 l / 100 km ; extra - urban 3.8 - 3.6 l / 100 km ; combined 4.2 - 4.1 l / 100 km , 95 - 93 g / km CO 2 ;"
```
Consumption[combined consumption 
	Vals[Val[[vfrom: 4.2, vto: 4.1] l100km]]
] 
Consumption[consumption 
	Vals[
		Val[urban [vfrom: 4.9, vto: 4.8] l100km] ; 
		Val[exurban [vfrom: 3.8, vto: 3.6] l100km] ; 
		Val[combined [vfrom: 4.2, vto: 4.1] l100km] ,
	]
] 
CO2Val[[vfrom: 95, vto: 93] gkm]
```

## authors
[Axaprj](https://github.com/Axaprj), [Igor Alexeev](mailto:axaprj2000@yahoo.com) 

You are welcome to [Property Indicators Lab](https://propertyindicators.github.io/)! 
We know how to use it in real projects.
For any questions, please contact us at email 
[propertyindicators@gmail.com](mailto:propertyindicators@gmail.com).