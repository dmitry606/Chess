(function () {

	self.Board = function (id, GamesService) {
		this.id = id;
		this.GamesService = GamesService;
	}

	Board.prototype = {
		update: function() {
			this.GamesService.getGame(this.id, this.getSafeSetGame());
		},

		move: function (from, to) {
			this.GamesService.makeMove(this.id, from, to, this.getSafeSetGame());
		},

		getMoves: function (pos, onSuccess) {
			this.GamesService.getMoves(this.id, pos, onSuccess);
		},

		getCurrentColor: function() {
			return this.currentGame.History.length % 2 == 0 ? 'White' : 'Black';
		},

		getSafeSetGame: function() {
			var me = this;
			return function (g) {
				me.setGame.call(me, g);
			}
		},

		setGame: function (updated) {
			if (!updated) return;
			var me = this;

			var allUpdated = updated.White.PieceStrings.
				map(function(m) { return m + 'White'}).
				concat(updated.Black.PieceStrings.
				map(function (m) { return m + 'Black' }));

			var allCurrent = me.currentGame? me.currentGame.White.PieceStrings.
				map(function(m) { return m + 'White'}).
				concat(me.currentGame.Black.PieceStrings.
				map(function(m) { return m + 'Black'})) : null;

			var removed = me.currentGame ? delta(allCurrent, allUpdated) : [];
			var added = me.currentGame ? delta(allUpdated, allCurrent) : allUpdated;

			removed.forEach(function (p) {
				delete me[p.substr(1, 2)];
			});
			
			added.forEach(function (p) {
				me[p.substr(1, 2)] = { color: p.substr(3, 5), pieceChar: p[0] };
			});

			me.currentGame = updated;
		}
	}
	
	function firstElemsEqual(a, b, n) {
		for (var i = 0; i < a.length && i < n; i++) {
			if (a[i] != b[i])
				return false;
		}

		return true;
	}

	function delta(li1, li2) {
		return li1.filter(function (el) {
			return !li2.includes(el);
		});
	}

})();