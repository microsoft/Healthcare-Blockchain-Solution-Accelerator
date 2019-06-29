import { TestBed } from '@angular/core/testing';

import { AutheticationService } from './authetication.service';

describe('AutheticationService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AutheticationService = TestBed.get(AutheticationService);
    expect(service).toBeTruthy();
  });
});
