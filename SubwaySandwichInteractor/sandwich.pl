member(X,[X|_]).
member(X,[_|R]) :- member(X,R).

options(X,meal) :- meals(L),member(X,L).
options(X,bread) :- breads(L),member(X,L).
options(X,main) :- \+selectedmeal(veggie),meatmains(L),member(X,L).
options(X,main) :- vegmains(L),member(X,L).
options(X,veggie) :- veggies(L),member(X,L).
options(X,sauce) :- \+selectedmeal(healthy),fattysauces(L),member(X,L).
options(X,sauce) :- nonfattysauces(L),member(X,L).
options(X,side) :- sides(L),member(X,L).
options(X,topup) :- \+selectedmeal(value),\+selectedmeal(vegan),nonvegantopups(L),member(X,L).
options(X,topup) :- \+selectedmeal(value),vegantopups(L),member(X,L).
options(X) :- options(X,_).

select(X,meal) :- \+selectedmeal(X),meals(L),member(X,L),assert(selectedmeal(X)).
select(X,bread) :- \+selectedbread(_),breads(L),member(X,L),assert(selectedbread(X)).
select(X,main) :- \+selectedmain(_),(
                        (\+selectedmeal(veggie),meatmains(L),member(X,L));
                        (vegmains(L),member(X,L))
                    ),assert(selectedmain(X)).
select(X,veggie) :- \+selectedveggie(X),veggies(L),member(X,L),assert(selectedveggie(X)).
select(X,sauce) :- \+selectedsauce(X),(
                         (\+selectedmeal(healthy),fattysauces(L),member(X,L));
                         (nonfattysauces(L),member(X,L))
                     ),assert(selectedsauce(X)).
select(X,side) :- \+selectedside(X),sides(L),member(X,L),assert(selectedside(X)).
select(X,topup) :- \+selectedtopup(X),\+selectedmeal(value),(
                         (\+selectedmeal(vegan),nonvegantopups(L),member(X,L));
                         (vegantopups(L),member(X,L))
                     ),assert(selectedtopup(X)).
select(X) :- select(X,_).

selected(X,meal) :- selectedmeal(X).
selected(X,bread) :- selectedbread(X).
selected(X,main) :- selectedmain(X).
selected(X,veggie) :- selectedveggie(X).
selected(X,sauce) :- selectedsauce(X).
selected(X,side) :- selectedside(X).
selected(X,topup) :- selectedtopup(X).
selected(X) :- selected(X,_).

meals([veggie,healthy,vegan,value]).
breads([wheat,honey_oat,italian,parmesan,flatbread]).
meatmains([chicken_and_bacon,chicken_teriyaki,cold_cut_trio,egg_mayo,italian_bmt,meatball_marinara_melt,roast_beef,roasted_chicken_breast,steak_and_cheese,subway_club,ham,subway_melt,tuna,turkey]).
vegmains([veggie_delite,veggie_patty]).
veggies([lettuce,green_peppers,red_onions,cucumbers,tomatoes,black_olives,jalapenos,pickles]).
fattysauces([mayonnaise,ranch,chipotle_southwest,barbecue]).
nonfattysauces([honey_mustard,sweet_onion]).
sides([soup,drinks,chips,cookies,hashbrowns,energy_bar,fruit_crisps,yogurt]).
vegantopups([veg_patty]).
nonvegantopups([cheese,bacon]).

reset :- retract(selectedmeal(_)),false.
reset :- retract(selectedbread(_)),false.
reset :- retract(selectedmain(_)),false.
reset :- retract(selectedveggie(_)),false.
reset :- retract(selectedsauce(_)),false.
reset :- retract(selectedside(_)),false.
reset :- retract(selectedtopup(_)),false.













